# Dokploy Deployment Guide

## Overview
This guide explains how to deploy the Badminton Forum application to Dokploy, a self-hosted PaaS platform.

## Architecture
- **Dokploy**: Self-hosted PaaS (similar to Heroku/Vercel)
- **Traefik**: Reverse proxy and load balancer
- **Docker Swarm**: Container orchestration
- **Server**: AWS EC2 (Osaka region - 56.155.38.231)

## File Structure
```
badminton-forum/
├── docker-compose.yml           # Local development configuration
├── docker-compose.dokploy.yml   # Dokploy deployment configuration
├── .env.dokploy.example        # Dokploy environment variables template
└── docs/
    └── DOKPLOY_DEPLOYMENT.md   # This file
```

## Key Differences: Local vs Dokploy

### 1. Network Configuration
- **Local**: Uses `badminton-network` (internal bridge network)
- **Dokploy**: Uses `dokploy-network` (external overlay network managed by Dokploy)

### 2. Port Exposure
- **Local**: Direct port mapping (e.g., `5173:5173`, `5246:5246`)
- **Dokploy**: Uses `expose` directive with Traefik labels for routing

### 3. Volume Mounts
- **Local**: Direct volume mounts for development
- **Dokploy**: Uses `../files/` prefix for persistent storage

### 4. Build Targets
- **Local**: Uses `development` target
- **Dokploy**: Uses `production` target

## Deployment Steps

### 1. Initial Setup in Dokploy

1. Log in to Dokploy: http://56.155.38.231:3000
2. Navigate to your project: `badminton-forum`
3. Go to the Compose service: `badminton-forum-stack`

### 2. Configure Environment Variables

In Dokploy's Environment tab, add:
```env
# Copy from .env.dokploy.example and modify as needed
MARIADB_PASSWORD=your-secure-password
JWT_SECRET=your-32-character-secret-key
# ... other variables
```

### 3. Update Docker Compose Configuration

In the Compose tab:
1. Change the compose file path from `docker-compose.yml` to `docker-compose.dokploy.yml`
2. Or paste the contents of `docker-compose.dokploy.yml` directly

### 4. Deploy

1. Click "Deploy" button
2. Monitor logs for any issues
3. Services will be available at:
   - Frontend: http://56.155.38.231
   - API: http://56.155.38.231/api
   - Admin: http://56.155.38.231/admin

## Accessing Services

### Without Domain (IP-based)
- **Frontend**: http://56.155.38.231
- **API**: http://56.155.38.231/api
- **Admin Panel**: http://56.155.38.231/admin

### With Domain (Future)
When you configure a domain:
1. Go to Domains tab in Dokploy
2. Add domain configurations for each service
3. Enable HTTPS with Let's Encrypt

## Troubleshooting

### Services Not Accessible
1. Check if containers are running:
   ```bash
   ssh ubuntu@56.155.38.231
   docker ps
   ```

2. Check Traefik configuration:
   ```bash
   docker logs dokploy-traefik
   ```

### Database Connection Issues
- Ensure MariaDB container is healthy
- Check environment variables in Dokploy
- Verify network connectivity between containers

### Build Failures
- Check available disk space
- Review build logs in Dokploy
- Ensure GitHub integration has access to repository

## Maintenance

### Viewing Logs
In Dokploy UI:
1. Go to your Compose service
2. Click on "Logs" tab
3. Select specific service to view logs

### Updating Application
1. Push changes to GitHub
2. Dokploy will auto-deploy if webhook is configured
3. Or manually click "Redeploy" in Dokploy UI

### Database Backups
SSH to server and run:
```bash
docker exec badminton-forum-db mysqldump -u badmintonuser -p badmintonforumdb > backup.sql
```

## Security Considerations

1. **Change default passwords** in production
2. **Use strong JWT secret** (at least 32 characters)
3. **Configure SSL/TLS** when domain is available
4. **Restrict SSH access** to specific IPs
5. **Regular backups** of database and uploaded files

## Performance Optimization

1. **Use production build targets** (already configured)
2. **Enable caching** in Traefik
3. **Configure health checks** for auto-recovery
4. **Monitor resource usage** via Dokploy dashboard

## Next Steps

1. **Configure custom domain** when available
2. **Set up SSL certificates** via Let's Encrypt
3. **Configure email service** for production
4. **Set up monitoring** and alerts
5. **Implement backup strategy**