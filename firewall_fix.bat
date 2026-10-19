@echo off
echo ============================================
echo   CampusQuest - Firewall Kural Ekleme
echo ============================================
echo.
echo Port 5105 icin guvenlik duvari kurali ekleniyor...
netsh advfirewall firewall add rule name="CampusQuest API Port 5105 IN" dir=in action=allow protocol=TCP localport=5105
netsh advfirewall firewall add rule name="CampusQuest API Port 5105 OUT" dir=out action=allow protocol=TCP localport=5105
echo.
echo Kural basariyla eklendi!
echo Artik telefonunuzdan API'ye erisebilirsiniz.
echo.
pause
