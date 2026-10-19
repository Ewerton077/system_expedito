# Configuração de Acesso Externo - System Expedito (VPS Ubuntu)

## 1. Configurar Nginx como Reverse Proxy

### Instalar Nginx
```bash
apt update && apt install -y nginx
```

### Configurar Site
```bash
cat > /etc/nginx/sites-available/santoexpedito << 'EOF'
server {
    listen 80;
    server_name SEU_DOMINIO_OU_IP;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
        proxy_read_timeout 300s;
        proxy_send_timeout 300s;
    }
}
EOF
```

### Ativar Site
```bash
ln -s /etc/nginx/sites-available/santoexpedito /etc/nginx/sites-enabled/
rm /etc/nginx/sites-enabled/default
nginx -t && systemctl reload nginx
```

---

## 2. Configurar HTTPS com Let's Encrypt (Certbot)

```bash
apt install -y certbot python3-certbot-nginx
certbot --nginx -d SEU_DOMINIO
# Seguir prompts (email, concordar termos, redirect HTTP->HTTPS)
```

**Renovação automática:**
```bash
systemctl enable certbot.timer
systemctl start certbot.timer
```

---

## 3. Configurar Firewall (UFW)

```bash
ufw allow OpenSSH
ufw allow 'Nginx Full'
ufw enable
ufw status
```

---

## 4. Configurar Serviço Systemd para o App

```bash
cat > /etc/systemd/system/santoexpedito.service << 'EOF'
[Unit]
Description=Santo Expedito ASP.NET Core App
After=network.target postgresql.service

[Service]
Type=notify
WorkingDirectory=/root/Desktop/system_expedito
ExecStart=/root/.dotnet/dotnet /root/Desktop/system_expedito/bin/Debug/net9.0/SantoExpedito.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=santoexpedito
User=root
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_ROOT=/root/.dotnet
Environment=PATH=/root/.dotnet:/root/.dotnet/tools:/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin

[Install]
WantedBy=multi-user.target
EOF
```

### Ativar e Iniciar
```bash
systemctl daemon-reload
systemctl enable santoexpedito
systemctl start santoexpedito
systemctl status santoexpedito
```

---

## 5. Configurar App para Produção

### appsettings.Production.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "SEU_DOMINIO",
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=santoexpedito;Username=postgres;Password=SUA_SENHA_FORTE"
  },
  "AdminPasswords": {
    "Casaderacao": "SENHA_FORTE_AQUI",
    "Evin": "OUTRA_SENHA_FORTE_AQUI"
  }
}
```

### Variáveis de Ambiente (recomendado para senhas)
```bash
cat > /etc/systemd/system/santoexpedito.service.d/override.conf << 'EOF'
[Service]
Environment=ADMIN_PASSWORD_CASADERACAO=senha_super_segura_123
Environment=ADMIN_PASSWORD_EVIN=outra_senha_super_segura_456
Environment=ASPNETCORE_URLS=http://localhost:5000
EOF
systemctl daemon-reload && systemctl restart santoexpedito
```

---

## 6. Verificar Tudo

```bash
# Status dos serviços
systemctl status santoexpedito nginx postgresql

# Logs da aplicação
journalctl -u santoexpedito -f

# Testar localmente
curl http://localhost:5000

# Testar via nginx
curl http://SEU_DOMINIO
curl https://SEU_DOMINIO
```

---

## 7. URLs Finais

- **HTTP:** `http://SEU_DOMINIO` (redireciona para HTTPS)
- **HTTPS:** `https://SEU_DOMINIO`
- **Swagger (se habilitado):** `https://SEU_DOMINIO/swagger`

---

## Comandos Úteis de Manutenção

```bash
# Rebuild e restart
cd /root/Desktop/system_expedito && dotnet publish -c Release && systemctl restart santoexpedito

# Ver logs nginx
tail -f /var/log/nginx/access.log
tail -f /var/log/nginx/error.log

# Backup database
pg_dump -U postgres santoexpedito > backup_$(date +%F).sql

# Restore database
psql -U postgres santoexpedito < backup_2026-10-13.sql
```