# WinPublicIP

[English](#english) | [Русский](#русский)

---

## English

A Windows system tray application that displays your current public IP address, country flag, and VPN status — inspired by the [KDE Public IP Address](https://store.kde.org/p/1395534) plasmoid.

### Screenshot

![KDE reference](https://raw.githubusercontent.com/topics/windows-tray/refs/heads/main/preview.png)

> Tray shows: `[country flag] x.x.x.x [VPN shield]`  
> Tooltip shows: IP, country, VPN status.

### Features

- **Country flag** — 60+ countries, loaded from embedded resources
- **Public IP** — displayed in tooltip and context menu; copied to clipboard in one click
- **VPN indicator** — green shield when VPN is active, grey when off
- **Auto-detection of VPN interfaces** — TAP, tun, WireGuard, OpenVPN, Tailscale, ZeroTier, PPP, IKEv2
- **Configurable refresh interval** — via Settings dialog (10–3600 sec)
- **Balloon notification** on IP change
- **Start with Windows** — optional autorun via registry
- **Self-contained single `.exe`** — no .NET installation required

### Requirements

- Windows 10 / 11 / Server 2019+
- x64

### Installation

1. Download `WinPublicIP.exe` from [Releases](../../releases).
2. Run it — the icon appears in the system tray immediately.
3. Optionally enable **Start with Windows** in the context menu.

### Build from source

```powershell
# Requires .NET 8 SDK: https://dotnet.microsoft.com/download/dotnet/8.0
git clone https://github.com/<your-username>/WinPublicIP.git
cd WinPublicIP
dotnet run                  # development
dotnet publish -c Release   # single-file exe → bin\Release\...\publish\WinPublicIP.exe
```

### Configuration

Settings are stored in `%APPDATA%\WinPublicIP\config.json` and can be edited manually:

```json
{
  "refreshIntervalSeconds": 60,
  "homeCountry": "RU",
  "vpnInterfacePatterns": ["TAP", "tun", "WireGuard", "OpenVPN", "Tailscale", "ZeroTier", "PPP", "WAN Miniport (IKEv2)"],
  "notifyOnIpChange": true,
  "startWithWindows": false
}
```

### VPN detection

The app scans active network interfaces for name/description patterns. Add your VPN adapter name to `vpnInterfacePatterns` in `config.json` if it is not detected automatically.

### Services used

| Purpose | Service | Notes |
|---------|---------|-------|
| External IP | [ipify.org](https://www.ipify.org) | Primary; fallback: ifconfig.me, icanhazip.com |
| Geolocation | [ip-api.com](http://ip-api.com) | Free, 45 req/min |
| Flag images | [flagcdn.com](https://flagcdn.com) | MIT, based on flag-icons |

### License

MIT — see [LICENSE](LICENSE).

---

## Русский

Приложение для системного трея Windows, которое отображает текущий публичный IP-адрес, флаг страны и статус VPN — по образцу плазмоида [KDE Public IP Address](https://store.kde.org/p/1395534).

### Возможности

- **Флаг страны** — 60+ стран, встроены в исполняемый файл
- **Внешний IP** — в tooltip и контекстном меню; копируется в буфер обмена одним кликом
- **Индикатор VPN** — зелёный щит при активном VPN, серый при отключённом
- **Авто-определение VPN-интерфейсов** — TAP, tun, WireGuard, OpenVPN, Tailscale, ZeroTier, PPP, IKEv2
- **Настраиваемый интервал обновления** — через диалог настроек (10–3600 сек)
- **Уведомление** при смене IP-адреса
- **Автозапуск с Windows** — через реестр, включается в меню
- **Один `.exe` без зависимостей** — установка .NET не требуется

### Системные требования

- Windows 10 / 11 / Server 2019+
- x64

### Установка

1. Скачать `WinPublicIP.exe` из раздела [Releases](../../releases).
2. Запустить — иконка появится в трее.
3. Опционально включить **Запускать с Windows** в контекстном меню.

### Сборка из исходников

```powershell
# Требуется .NET 8 SDK: https://dotnet.microsoft.com/download/dotnet/8.0
git clone https://github.com/<your-username>/WinPublicIP.git
cd WinPublicIP
dotnet run                  # режим разработки
dotnet publish -c Release   # одиночный exe → bin\Release\...\publish\WinPublicIP.exe
```

### Конфигурация

Настройки хранятся в `%APPDATA%\WinPublicIP\config.json`:

```json
{
  "refreshIntervalSeconds": 60,
  "homeCountry": "RU",
  "vpnInterfacePatterns": ["TAP", "tun", "WireGuard", "OpenVPN", "Tailscale", "ZeroTier", "PPP", "WAN Miniport (IKEv2)"],
  "notifyOnIpChange": true,
  "startWithWindows": false
}
```

### Определение VPN

Приложение сканирует активные сетевые интерфейсы по имени и описанию. Если ваш VPN-адаптер не определяется — добавьте его имя в `vpnInterfacePatterns` в `config.json`.

### Используемые сервисы

| Назначение | Сервис | Примечания |
|-----------|--------|------------|
| Внешний IP | [ipify.org](https://www.ipify.org) | Основной; fallback: ifconfig.me, icanhazip.com |
| Геолокация | [ip-api.com](http://ip-api.com) | Бесплатно, 45 запросов/мин |
| Иконки флагов | [flagcdn.com](https://flagcdn.com) | MIT, основан на flag-icons |

### Лицензия

MIT — см. [LICENSE](LICENSE).
