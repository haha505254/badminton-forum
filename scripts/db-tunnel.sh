#!/bin/bash

# 資料庫連接資訊
echo "==================================="
echo "PostgreSQL 資料庫連接資訊"
echo "==================================="
echo ""
echo "1. 先執行此腳本建立 SSH 隧道"
echo "2. 在 GUI 工具中使用以下資訊連接："
echo ""
echo "Host: localhost"
echo "Port: 5433"
echo "Database: badmintonforumdb"
echo "Username: badmintonuser"
echo "Password: BadmintonPass123"
echo ""
echo "==================================="
echo ""
echo "正在建立 SSH 隧道..."
echo "按 Ctrl+C 結束連接"
echo ""

# 建立 SSH 隧道
ssh -i /home/one123/my-ec2-key.pem -L 5433:localhost:5432 ubuntu@34.199.113.111 -N