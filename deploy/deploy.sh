#!/bin/bash

# Badminton Forum Deployment Script
# This script is used by GitHub Actions to deploy the application to EC2

set -e

echo "Starting deployment..."

# Variables
ECR_URI="814247511761.dkr.ecr.us-east-1.amazonaws.com"
API_IMAGE="$ECR_URI/badminton-forum-api:latest"
FRONTEND_IMAGE="$ECR_URI/badminton-forum-frontend:latest"

# Get parameters from Systems Manager
echo "Fetching configuration from AWS Systems Manager..."
export DB_PASSWORD=$(aws ssm get-parameter --name "/badminton-forum/db-password" --with-decryption --query 'Parameter.Value' --output text)
export JWT_SECRET=$(aws ssm get-parameter --name "/badminton-forum/jwt-secret" --with-decryption --query 'Parameter.Value' --output text)

# Login to ECR
echo "Logging in to ECR..."
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin $ECR_URI

# Pull latest images
echo "Pulling latest images..."
docker pull $API_IMAGE
docker pull $FRONTEND_IMAGE

# Navigate to deployment directory
cd ~/badminton-forum

# Update containers with zero downtime
echo "Updating containers..."
docker-compose -f docker-compose.prod.yml up -d --remove-orphans

# Wait for services to be healthy
echo "Waiting for services to be healthy..."
sleep 30

# Check service health
echo "Checking service health..."
docker-compose -f docker-compose.prod.yml ps

# Database migrations
# Note: Production container no longer includes EF tools for security and size optimization
# Migrations should be handled separately, e.g.:
# - Use a dedicated migration container during deployment
# - Run migrations from CI/CD pipeline before deployment
# - Or manually run migrations when needed
echo "Skipping database migrations (should be handled separately)"

# Clean up old images
echo "Cleaning up old images..."
docker image prune -f

echo "Deployment completed successfully!"