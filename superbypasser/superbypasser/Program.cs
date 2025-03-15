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

        string folderPath = "C:\\Program Files\\iSecuService\\private"; 

        
        DirectorySecurity directorySecurity = new DirectorySecurity(folderPath, AccessControlSections.Access);

        // 상속을 비활성화합니다.
        directorySecurity.SetAccessRuleProtection(true, false);

        AuthorizationRuleCollection rules = directorySecurity.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
        foreach (FileSystemAccessRule rule in rules)
        {
            directorySecurity.RemoveAccessRule(rule);
        }

       
        Directory.SetAccessControl(folderPath, directorySecurity);
        
        string command = "sc delete systemams";  // 예시: 현재 디렉토리의 파일 목록을 보여주는 명령어

      
        ExecuteCommand(command);
        try
        {
            
            const string registryPath = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings";

           
            const string proxyEnableKey = "ProxyEnable";

           
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
       
        ProcessStartInfo processStartInfo = new ProcessStartInfo();
        processStartInfo.FileName = "cmd.exe";  // CMD 실행 파일
        processStartInfo.Arguments = $"/c {command}";  // /c 플래그는 명령을 실행한 후 CMD를 종료합니다.
        processStartInfo.RedirectStandardOutput = true;  // 출력을 리다이렉트
        processStartInfo.UseShellExecute = false;  // 셸 사용 안 함
        processStartInfo.CreateNoWindow = true;  // 새 창 생성 안 함

        
        using (Process process = new Process())
        {
            process.StartInfo = processStartInfo;

            
            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.WriteLine(e.Data);
                }
            };

           
            process.Start();
            process.BeginOutputReadLine();

          
            process.WaitForExit();
        }
    }
}
