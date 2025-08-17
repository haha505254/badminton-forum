#!/bin/bash

# Badminton Forum Production Deployment Script
# This script automates the deployment process on EC2 or any production server

set -e  # Exit on error

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

# Function to check if .env file exists and is configured
check_env_file() {
    if [ ! -f .env ]; then
        print_warning ".env file not found. Creating from defaults..."
        cp .env.defaults .env
        print_error "Please edit .env file with your production settings:"
        echo "  - SERVER_IP: Your server's public IP"
        echo "  - ALLOWED_ORIGINS: Update with your server IP"
        echo "  - VITE_API_URL: Update with your server IP"
        echo "  - JWT_SECRET: Change to a secure secret key"
        echo "  - MARIADB_PASSWORD: Change to a secure password"
        echo "  - DEFAULT_ADMIN_EMAIL: Set your admin email"
        echo "  - DEFAULT_ADMIN_PASSWORD: Set a secure admin password"
        echo ""
        echo "After editing, run this script again."
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
    
    # Build images with no cache to ensure fresh build
    print_info "Building Docker images (this may take a few minutes)..."
    docker compose -f docker-compose.prod.yml build --no-cache
    
    # Start services in detached mode
    print_info "Starting services..."
    docker compose -f docker-compose.prod.yml up -d
    
    # Wait for services to be healthy
    print_info "Waiting for services to be ready..."
    sleep 10
    
    # Check service status
    docker compose -f docker-compose.prod.yml ps
}

# Function to verify deployment
verify_deployment() {
    print_info "Verifying deployment..."
    
    # Check if API is responding
    if curl -f -s -o /dev/null http://localhost:5246/api/health 2>/dev/null; then
        print_info "✓ API is responding"
    else
        print_warning "API health check failed. Checking logs..."
        docker compose -f docker-compose.prod.yml logs --tail=20 api
    fi
    
    # Check if web frontend is responding
    if curl -f -s -o /dev/null http://localhost:5173 2>/dev/null; then
        print_info "✓ Web frontend is responding"
    else
        print_warning "Web frontend check failed"
    fi
    
    # Check if admin panel is responding
    if curl -f -s -o /dev/null http://localhost:5174 2>/dev/null; then
        print_info "✓ Admin panel is responding"
    else
        print_warning "Admin panel check failed"
    fi
    
    # Check database connection
    if docker compose -f docker-compose.prod.yml exec -T db mariadb -u badmintonuser -pBadmintonPass123 -e "SELECT 1" badmintonforumdb &>/dev/null; then
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