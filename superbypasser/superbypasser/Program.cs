using System;
using System.Security.AccessControl;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;


class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Made by ETINILA");

        string folderPath = "C:\\Program Files\\iSecuService\\private"; // 대상 폴더 경로를 지정합니다.

        // 폴더의 현재 ACL을 가져옵니다.
        DirectorySecurity directorySecurity = new DirectorySecurity(folderPath, AccessControlSections.Access);

        // 상속을 비활성화합니다.
        directorySecurity.SetAccessRuleProtection(true, false);

        AuthorizationRuleCollection rules = directorySecurity.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
        foreach (FileSystemAccessRule rule in rules)
        {
            directorySecurity.RemoveAccessRule(rule);
        }

        // 변경된 ACL을 폴더에 적용합니다.
        Directory.SetAccessControl(folderPath, directorySecurity);
        // 실행할 CMD 명령어
        string command = "sc delete systemams";  // 예시: 현재 디렉토리의 파일 목록을 보여주는 명령어

        // 프로세스 시작
        ExecuteCommand(command);
        try
        {
            // 프록시 설정이 저장된 레지스트리 경로
            const string registryPath = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings";

            // 프록시 사용 여부를 나타내는 레지스트리 키
            const string proxyEnableKey = "ProxyEnable";

            // 프록시 사용 여부를 0으로 설정하여 비활성화
            Registry.SetValue(registryPath, proxyEnableKey, 0, RegistryValueKind.DWord);

            Console.WriteLine("\n프록시 설정이 비활성화되었습니다.\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"오류: {ex.Message}");
        }

        Console.WriteLine("우회 완료");
        Console.ReadKey();
    }

    static void ExecuteCommand(string command)
    {
        // 프로세스 시작 정보 설정
        ProcessStartInfo processStartInfo = new ProcessStartInfo();
        processStartInfo.FileName = "cmd.exe";  // CMD 실행 파일
        processStartInfo.Arguments = $"/c {command}";  // /c 플래그는 명령을 실행한 후 CMD를 종료합니다.
        processStartInfo.RedirectStandardOutput = true;  // 출력을 리다이렉트
        processStartInfo.UseShellExecute = false;  // 셸 사용 안 함
        processStartInfo.CreateNoWindow = true;  // 새 창 생성 안 함

        // 프로세스 시작
        using (Process process = new Process())
        {
            process.StartInfo = processStartInfo;

            // 이벤트 핸들러를 등록하여 출력을 읽음
            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.WriteLine(e.Data);
                }
            };

            // 프로세스 시작 및 비동기적으로 출력 읽기 시작
            process.Start();
            process.BeginOutputReadLine();

            // 프로세스가 완료될 때까지 기다림
            process.WaitForExit();
        }
    }
}
