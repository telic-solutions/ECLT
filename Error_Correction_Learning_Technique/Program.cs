﻿using MaterialSkin.Controls;
using Squirrel;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Error_Correction_Learning_Technique
{
    static class Program
    {
        private static bool ShowTheWelcomeWizard;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            CheckForUpdates().Wait(2);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

        }

        private static async Task CheckForUpdates()
        {
            var upgraded = false;
            var mgr = UpdateManager.GitHubUpdateManager("https://github.com/telic-solutions/ECLT");

            while (!upgraded)
            {
                await Task.Delay(TimeSpan.FromSeconds(5));

                try
                {
                    using (UpdateManager result = await mgr)
                    {
                        SquirrelAwareApp.HandleEvents(
                                        onInitialInstall: v => mgr.Result.CreateShortcutForThisExe(),
                                        onAppUpdate: v => mgr.Result.CreateShortcutForThisExe(),
                                        onAppUninstall: v => mgr.Result.RemoveShortcutForThisExe(),
                                        onFirstRun: () => ShowTheWelcomeWizard = true);

                        await result.UpdateApp();
                        upgraded = true;

                        //ReleaseEntry release = await mgr.Result.UpdateApp();
                        // string x = $"PackageName : {release.PackageName}\nBaseURL {release.BaseUrl}\nFiles:{release.Filename}\nFileSize:{release.Filesize}\nVer:{release.Version}";
                    }
                    mgr.Dispose();
                }
                catch (Exception ex)
                {
                    string message = ex.Message + Environment.NewLine;
                    if (ex.InnerException != null)
                        message += ex.InnerException.Message;
                    MaterialMessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
