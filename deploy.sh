#!/bin/bash

# Badminton Forum Production Deployment Script
# This script automates the deployment process on EC2 or any production server
#
# Usage:
#   ./deploy.sh           # Normal deployment (with cache, fast)
#   ./deploy.sh --force   # Force rebuild (no cache, slow but guaranteed fresh)

set -e  # Exit on error
set -u  # Exit on undefined variable
set -o pipefail  # Exit on pipe failure

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to print colored output
print_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Error handler
trap 'print_error "Deployment failed at line $LINENO"' ERR

# Function to check if .env file exists and is configured
check_env_file() {
    if [ ! -f .env ]; then
        print_warning ".env file not found. Creating from defaults..."
        cp .env.defaults .env
        print_error "Please edit .env file with your production settings:"
        echo "  - SERVER_IP: Your server's public IP"
        echo "  - VITE_API_URL: Update with your server IP"
        echo "  - ALLOWED_ORIGINS: Update with your server IP"
        echo "  - EMAIL_BASE_URL: Update with your server IP"
        echo "  - JWT_SECRET: Change to a secure secret key"
        echo "  - MARIADB_PASSWORD: Change to a secure password"
        echo "  - DEFAULT_ADMIN_EMAIL: Set your admin email"
        echo "  - DEFAULT_ADMIN_PASSWORD: Set a secure admin password"
        echo ""
        echo "After editing, run this script again."
        exit 1
    fi
    
    # Check if VITE_API_URL still points to localhost
    if grep -q "VITE_API_URL=http://localhost" .env; then
        print_error "VITE_API_URL still points to localhost! Please update to your server IP."
        echo "Current value:"
        grep VITE_API_URL .env
        exit 1
    fi
    
    # Check if default passwords are still being used
    if grep -q "BadmintonPass123" .env; then
        print_warning "Default database password detected. Please change MARIADB_PASSWORD in .env for production!"
    fi
    
    if grep -q "ThisIsAVerySecretKeyForJWTTokenGenerationPleaseChangeInProduction" .env; then
        print_warning "Default JWT secret detected. Please change JWT_SECRET in .env for production!"
    fi
}

# Function to update code from git
update_code() {
    print_info "Checking for code updates..."
    
    # Check if git repo
    if [ -d .git ]; then
        # Stash any local changes
        git stash
        
        # Pull latest changes
        print_info "Pulling latest changes from repository..."
        git pull origin main || git pull origin master
        
        print_info "Code updated successfully"
    else
        print_warning "Not a git repository. Skipping code update."
    fi
}

# Function to stop existing containers
stop_containers() {
    print_info "Stopping existing containers..."
    
    # Check if any containers are running
    if docker compose -f docker-compose.prod.yml ps -q 2>/dev/null | grep -q .; then
        docker compose -f docker-compose.prod.yml down
        print_info "Existing containers stopped"
    else
        print_info "No existing containers found"
    fi
}

# Function to clean up Docker resources
cleanup_docker() {
    print_info "Cleaning up Docker resources..."
    
    # Remove dangling images
    docker image prune -f
    
    # Remove unused networks
    docker network prune -f
    
    print_info "Docker cleanup completed"
}

# Function to build and start services
deploy_services() {
    print_info "Building and starting services..."
    
    # Check for --force flag and build accordingly
    if [[ "${FORCE_REBUILD:-}" == "true" ]]; then
        print_info "Force rebuild requested - building without cache"
        print_info "This will take longer but ensures all images are freshly built..."
        docker compose -f docker-compose.prod.yml build --no-cache
        docker compose -f docker-compose.prod.yml up -d
    else
        print_info "Building Docker images and starting services (this may take a few minutes)..."
        docker compose -f docker-compose.prod.yml up -d --build
    fi
    
    # Wait for services to be healthy
    print_info "Waiting for services to be ready"
    
    # Wait for API to be healthy with smart retry logic
    local max_wait=60
    local elapsed=0
    local interval=5
    
    printf "   Waiting for API health check"
    while [ $elapsed -lt $max_wait ]; do
        if docker compose -f docker-compose.prod.yml ps api 2>/dev/null | grep -q "(healthy)"; then
            printf " ✓\n"
            print_info "API is healthy"
            break
        fi
        printf "."
        sleep $interval
        elapsed=$((elapsed + interval))
    done
    
    if [ $elapsed -ge $max_wait ]; then
        printf " ✗\n"
        print_warning "API did not become healthy within ${max_wait} seconds"
        print_info "Checking API logs for issues..."
        docker compose -f docker-compose.prod.yml logs --tail=30 api
    fi
    
    # Check service status
    docker compose -f docker-compose.prod.yml ps
}

# Function to verify deployment
verify_deployment() {
    print_info "Verifying deployment..."
    
    # Source .env to get variables
    source .env
    
    # Check if API is responding with retry logic
    local api_ready=false
    local max_attempts=12
    local attempt=0
    
    printf "   Checking API endpoint"
    while [ $attempt -lt $max_attempts ]; do
        if curl -f -s -o /dev/null http://localhost:5246/health 2>/dev/null; then
            api_ready=true
            printf " ✓\n"
            print_info "API is responding"
            break
        fi
        attempt=$((attempt + 1))
        if [ $attempt -lt $max_attempts ]; then
            printf "."
            sleep 5
        fi
    done
    
    if [ "$api_ready" = false ]; then
        printf " ✗\n"
        print_warning "API health check failed after ${max_attempts} attempts"
        print_info "Checking API logs for issues..."
        docker compose -f docker-compose.prod.yml logs --tail=30 api
    fi
    
    # Check web frontend container and Nginx
    printf "   Checking Web frontend"
    local web_attempts=0
    local web_ready=false
    while [ $web_attempts -lt 3 ]; do
        if docker compose -f docker-compose.prod.yml ps web 2>/dev/null | grep -q "Up"; then
            if docker compose -f docker-compose.prod.yml exec -T web wget -qO- http://localhost:80 > /dev/null 2>&1; then
                web_ready=true
                printf " ✓\n"
                print_info "Web frontend is running"
                break
            fi
        fi
        web_attempts=$((web_attempts + 1))
        if [ $web_attempts -lt 3 ]; then
            printf "."
            sleep 2
        fi
    done
    if [ "$web_ready" = false ]; then
        printf " ✗\n"
        print_warning "Web frontend is not responding properly"
    fi
    
    # Check admin panel container and Nginx
    printf "   Checking Admin panel"
    local admin_attempts=0
    local admin_ready=false
    while [ $admin_attempts -lt 3 ]; do
        if docker compose -f docker-compose.prod.yml ps admin 2>/dev/null | grep -q "Up"; then
            if docker compose -f docker-compose.prod.yml exec -T admin wget -qO- http://localhost:80 > /dev/null 2>&1; then
                admin_ready=true
                printf " ✓\n"
                print_info "Admin panel is running"
                break
            fi
        fi
        admin_attempts=$((admin_attempts + 1))
        if [ $admin_attempts -lt 3 ]; then
            printf "."
            sleep 2
        fi
    done
    if [ "$admin_ready" = false ]; then
        printf " ✗\n"
        print_warning "Admin panel is not responding properly"
    fi
    
    # Check database connection using variables from .env
    if docker compose -f docker-compose.prod.yml exec -T db mariadb \
        -u ${MARIADB_USER:-badmintonuser} \
        -p${MARIADB_PASSWORD:-BadmintonPass123} \
        -e "SELECT 1" ${MARIADB_DATABASE:-badmintonforumdb} &>/dev/null; then
        print_info "✓ Database is accessible"
    else
        print_warning "Database connection check failed"
    fi
}

# Function to show deployment summary
show_summary() {
    echo ""
    echo "========================================="
    echo "   Deployment Complete!"
    echo "========================================="
    echo ""
    echo "Services are available at:"
    echo "  - Main Forum: http://$(hostname -I | awk '{print $1}'):5173"
    echo "  - Admin Panel: http://$(hostname -I | awk '{print $1}'):5174"
    echo "  - API: http://$(hostname -I | awk '{print $1}'):5246/swagger"
    echo ""
    echo "Useful commands:"
    echo "  - View logs: docker compose -f docker-compose.prod.yml logs -f [service]"
    echo "  - Restart service: docker compose -f docker-compose.prod.yml restart [service]"
    echo "  - Stop all: docker compose -f docker-compose.prod.yml down"
    echo "  - Service status: docker compose -f docker-compose.prod.yml ps"
    echo ""
    
    # Show warnings if any
    if grep -q "BadmintonPass123\|ThisIsAVerySecretKeyForJWTTokenGenerationPleaseChangeInProduction" .env; then
        echo "⚠️  SECURITY WARNING:"
        echo "   Please change default passwords in .env file!"
        echo ""
    fi
}

# Main deployment process
main() {
    echo "========================================="
    echo "   Badminton Forum Deployment Script"
    echo "========================================="
    echo ""
    
    # Check for --force flag
    if [[ "${1:-}" == "--force" ]]; then
        FORCE_REBUILD=true
        print_info "Force rebuild mode enabled"
    else
        FORCE_REBUILD=false
    fi
    
    # Step 1: Check environment
    print_info "Step 1: Checking environment configuration..."
    check_env_file
    
    # Step 2: Update code
    print_info "Step 2: Updating code..."
    update_code
    
    # Step 3: Stop existing containers
    print_info "Step 3: Stopping existing services..."
    stop_containers
    
    # Step 4: Clean up Docker
    print_info "Step 4: Cleaning up Docker resources..."
    cleanup_docker
    
    # Step 5: Deploy services
    print_info "Step 5: Deploying services..."
    deploy_services
    
    # Step 6: Verify deployment
    print_info "Step 6: Verifying deployment..."
    verify_deployment
    
    # Show summary
    show_summary
}

# Run main function
main "$@"