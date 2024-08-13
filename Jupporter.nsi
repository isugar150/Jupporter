; Script generated by the HM NIS Edit Script Wizard.

; HM NIS Edit Wizard helper defines
!define PRODUCT_NAME "Jupporter"
!define PRODUCT_VERSION "1.1"
!define PRODUCT_PUBLISHER "Jm's Corp"
!define PRODUCT_WEB_SITE "https://www.namejm.com/"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\Jupporter.exe"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"

; MUI 1.67 compatible ------
!include "MUI.nsh"

; MUI Settings
!define MUI_ABORTWARNING
!define MUI_ICON "${NSISDIR}\Contrib\Graphics\Icons\modern-install.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"

; Welcome page
!insertmacro MUI_PAGE_WELCOME
; License page
!insertmacro MUI_PAGE_LICENSE "LICENSE.txt"
; Directory page
!insertmacro MUI_PAGE_DIRECTORY
; Instfiles page
!insertmacro MUI_PAGE_INSTFILES
; Finish page
!insertmacro MUI_PAGE_FINISH

; Uninstaller pages
!insertmacro MUI_UNPAGE_INSTFILES

; Language files
!insertmacro MUI_LANGUAGE "Korean"

; MUI end ------

Name "${PRODUCT_NAME}"
OutFile "Jupporter Setup.exe"
InstallDir "C:\Jupporter"
InstallDirRegKey HKLM "${PRODUCT_DIR_REGKEY}" ""
ShowInstDetails show
ShowUnInstDetails show

Section "MainSection" SEC01
  SetOutPath "$INSTDIR"
  ExecWait "cmd.exe /c taskkill /f /im Jupporter.exe /t & timeout 1"
  SetOverwrite on

  File ".\Jupporter\bin\Release\Jupporter.exe"
  File ".\Jupporter\bin\Release\log4net.dll"
  File ".\Jupporter\bin\Release\log4net.xml"
  File ".\Jupporter\bin\Release\NLog.dll"
  File ".\Jupporter\bin\Release\NLog.xml"

  CreateDirectory "$SMPROGRAMS\Jupporter"
  CreateShortCut "$SMPROGRAMS\Jupporter\Jupporter.lnk" "$INSTDIR\Jupporter.exe"
  CreateShortCut "$DESKTOP\Jupporter.lnk" "$INSTDIR\Jupporter.exe"
SectionEnd

Section -AdditionalIcons
  CreateShortCut "$SMPROGRAMS\Jupporter\Uninstall.lnk" "$INSTDIR\uninst.exe"
SectionEnd

Section -Post
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\Jupporter.exe"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Run" "Jupporter" '"$InstDir\Jupporter.exe" /autostart'
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\Jupporter.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
SectionEnd


Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name)  (  )           ŵǾ    ϴ ."
FunctionEnd

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "$(^Name)  (  )      Ͻðڽ  ϱ ?" IDYES +2
  Abort
FunctionEnd

Section Uninstall
  SetOutPath "$INSTDIR"
  ExecWait "cmd.exe /c taskkill /f /im Jupporter.exe /t & timeout 1"

  Delete "$INSTDIR\uninst.exe"
  Delete "$INSTDIR\Jupporter.exe"
  Delete "$INSTDIR\log4net.dll"
  Delete "$INSTDIR\log4net.xml"
  Delete "$INSTDIR\NLog.dll"
  Delete "$INSTDIR\NLog.xml"
  Delete "$INSTDIR\Jupporter.ini"

  Delete "$SMPROGRAMS\Uninstall.lnk"
  Delete "$DESKTOP\Jupporter.lnk"
  Delete "$SMPROGRAMS\Jupporter\Jupporter.lnk"

  RMDir "$SMPROGRAMS\Jupporter"
  RMDir "$INSTDIR\nl"
  RMDir "$INSTDIR\API"
  RMDir "$INSTDIR"

  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKLM "${PRODUCT_DIR_REGKEY}"
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\RunJupporter"
  SetAutoClose true
SectionEnd