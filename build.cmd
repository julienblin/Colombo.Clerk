@echo off

lib\NAnt\NAnt.exe -buildfile:build\Colombo.Clerk.build  -logger:NAnt.LoggerExtension.ConsoleColorLogger build

if %ERRORLEVEL% NEQ 0 goto errors

goto finish

:errors
pause

:finish