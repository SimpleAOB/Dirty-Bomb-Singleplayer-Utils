using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DBSPUtils
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
        }
        public bool launching = true;
        private bool SCDialogOpen = false;
        private String consoleLastMsg = "";
        private Dictionary<int, string> classes = new Dictionary<int, string>();

        private Dictionary<int, string> weapons = new Dictionary<int, string>();

        private Dictionary<int, string> items = new Dictionary<int, string>();
        private Dictionary<int, string> maps = new Dictionary<int, string>();
        private Dictionary<string, string> map_classes = new Dictionary<string, string>();
        private Dictionary<string, string> map_weapons = new Dictionary<string, string>();
       
        public void tConsole(Object write)
        {
            consoleLastMsg = "<" + DateTime.Now.ToString() + "> " + write.ToString();
            string bt;
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
            {
                bt = console.Text;
                string fullc;
                if (write.ToString() != "Application loaded successfully")
                {
                    fullc = bt + Environment.NewLine + consoleLastMsg;
                }
                else
                {
                    fullc = consoleLastMsg;
                }
                SetText(fullc);
            }));
            Console.WriteLine("<" + DateTime.Now.ToString() + "> " + write.ToString());
        }
        private void SetText(string text)
        {
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
            {
                console.AppendText(Environment.NewLine + consoleLastMsg);
                console.ScrollToEnd();
            }));
        }
        private void WriteResources()
        {
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
            {
                commandBuilder.Text = "Checking Requires Files...";
            }));
            if (!Properties.Settings.Default.quick_launch)
            {
                string filepath = Directory.GetCurrentDirectory() + "\\extract.exe";
                tConsole("Checking if required files are present");
                if (!File.Exists(filepath))
                {
                    try
                    {
                        tConsole("'extract.exe' not found");
                        Assembly assm = Assembly.GetCallingAssembly();
                        tConsole("Getting 'extract.exe' from resources");
                        using (Stream s = assm.GetManifestResourceStream("DBSPUtils.extract.exe"))
                        using (BinaryReader r = new BinaryReader(s))
                        using (FileStream fs = new FileStream(filepath, FileMode.CreateNew))
                        using (BinaryWriter w = new BinaryWriter(fs))
                            w.Write(r.ReadBytes((int)s.Length));
                        tConsole("Retreival of 'extract.exe' successful");
                    }
                    catch (Exception ex)
                    {
                        tConsole("Retreival of 'extract.exe' failed. Please send the following line to a developer for assistance");
                        tConsole(ex.InnerException.ToString());
                    }
                }
                else
                {
                    tConsole("All required files are present");
                }
            }
        }
        private bool findDBFiles()
        {
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
            {
                commandBuilder.Text = "Looking for Steam/Dirty Bomb files";
            }));
            //We will check on start up each time incase the files get moved to another hard drive or folder location
            //Can change using settings in app
            var sp = Properties.Settings.Default.steam_path;
            tConsole("Testing for Steam folder. Looking for 'SteamApps\\common'");
            if (Directory.Exists(sp + @"\SteamApps\common"))
            {
                tConsole("Steam folder found. Testing for Dirty Bomb files");
                //Test using Engineer_01 because I am biased
                var tl = @"\SteamApps\common\Dirty Bomb\ShooterGame\CookedPC\Characters\Engineer_01";
                if (Directory.Exists(sp + tl))
                {
                    tConsole("Game files found");
                    return true;
                }
                else
                {
                    tConsole("Game files not found. Please install Dirty Bomb");
                    tConsole("Is our analysis wrong? Message the developers of this application to get it fixed");
                    return false;
                }
            }
            else
            {
                tConsole("Steam folder not found");
                string sMessageBoxText = "Do you wish to manually select the Steam location?";
                string sCaption = "Steam Folder Not Found";

                MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;
                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                MessageBoxResult rsltMessageBox = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

                switch (rsltMessageBox)
                {
                    case MessageBoxResult.Yes:
                        var dialog = new CommonOpenFileDialog();
                        dialog.IsFolderPicker = true;
                        CommonFileDialogResult result = dialog.ShowDialog();
                        Properties.Settings.Default.steam_path = dialog.FileNames.ElementAt(0);
                        Properties.Settings.Default.Save();
                        tConsole("Selected folder: " + Properties.Settings.Default.steam_path);
                        break;

                    case MessageBoxResult.No:
                        break;
                }
                return false;
            }
        }

        private void enumDicts()
        {
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
            {
                commandBuilder.Text = "Building Internal Database...1/2";
            }));
            var i = 0;
            foreach (string d in Directory.GetDirectories(Properties.Settings.Default.steam_path + @"\SteamApps\common\Dirty Bomb\ShooterGame\CookedPC\Characters"))
            {
                var chr = d.Substring(d.LastIndexOf('\\') + 1);
                tConsole("Character: " + chr);
                classes.Add(i, chr);
                i++;
            }
            i = 0;
            foreach (string d in Directory.GetDirectories(Properties.Settings.Default.steam_path + @"\SteamApps\common\Dirty Bomb\ShooterGame\CookedPC\Weapons"))
            {
                var wep = d.Substring(d.LastIndexOf('\\') + 1);
                tConsole("Weapon: " + wep);
                weapons.Add(i, wep);
                i++;
            }
            i = 0;
            foreach (string d in Directory.GetDirectories(Properties.Settings.Default.steam_path + @"\SteamApps\common\Dirty Bomb\ShooterGame\CookedPC\Items"))
            {
                var item = d.Substring(d.LastIndexOf('\\') + 1);
                tConsole("Item: " + item);
                items.Add(i, item);
                i++;
            }
            i = 0;
            foreach (string d in Directory.GetDirectories(Properties.Settings.Default.steam_path + @"\SteamApps\common\Dirty Bomb\ShooterGame\CookedPC\Maps"))
            {
                var map = d.Substring(d.LastIndexOf('\\') + 1);
                tConsole("Map: " + map);
                maps.Add(i, map);
                i++;
            }
        }

        private void enumMapDicts()
        {
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
            {
                commandBuilder.Text = "Building Internal Database...2/2";
            }));
            tConsole("Starting Mapping");
            Directory.CreateDirectory("temp");
            if (!Properties.Settings.Default.quick_launch)
            {
                foreach (KeyValuePair<int, string> ent in classes)
                {
                    try
                    {
                        var loc1 = Properties.Settings.Default.steam_path + @"\SteamApps\common\Dirty Bomb\ShooterGame\CookedPC\Characters\" + ent.Value + "\\" + ent.Value + "_Frontend_SF.upk";
                        var loc2 = "temp\\" + ent.Value + @"_Frontend_SF.upk";
                        var dirLoc1 = ent.Value + "_Frontend_SF";
                        var dirLoc2 = "temp\\" + ent.Value + "_Frontend_SF";
                        var ntloc1 = @"temp\" + ent.Value + @"_Frontend_SF\NameTable.txt";
                        if (ent.Value == "Engineer_01")
                        {
                            loc1 = Properties.Settings.Default.steam_path + @"\SteamApps\common\Dirty Bomb\ShooterGame\CookedPC\Characters\" + ent.Value + "\\" + "E01_Frontend_SF.upk";
                            loc2 = "temp\\E01_Frontend_SF.upk";
                            dirLoc1 = "E01_Frontend_SF";
                            dirLoc2 = "temp\\E01_Frontend_SF";
                            ntloc1 = @"temp\E01_Frontend_SF\NameTable.txt";
                        }

                        tConsole("Copying " + ent.Value);

                        File.Copy(loc1, loc2, true);
                        tConsole("Unpacking " + ent.Value);

                        Process ext = new Process();
                        ext.StartInfo.FileName = "extract.exe";
                        ext.StartInfo.Arguments = loc2;
                        ext.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        ext.Start();
                        ext.WaitForExit();

                        Directory.Move(dirLoc1, dirLoc2);

                        tConsole("Finding Merc name");
                        string name;
                        using (StreamReader r = new StreamReader(ntloc1))
                        {
                            string line;
                            while ((line = r.ReadLine()) != null)
                            {
                                if (line.Contains("_small_canvas"))
                                {
                                    var stname = line.IndexOf('"') + 1;
                                    string subname = line.Substring(stname);
                                    var endname = subname.IndexOf('_');
                                    name = subname.Substring(0, endname);
                                    name = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(name);
                                    tConsole(ent.Value + " = " + name);
                                    map_classes.Add(name, ent.Value);
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        tConsole(ex.Message);
                        tConsole(ex.StackTrace);
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<int, string> ent in classes)
                {
                    map_classes.Add(ent.Value, ent.Value);
                }
            }
            //Map weapons manually
            map_weapons["AssaultRifle_01"] = "M4A1";
            map_weapons["AssaultRifle_02"] = "BR-16";
            map_weapons["AssaultRifle_03"] = "Dreiss AR";
            map_weapons["AssaultRifle_04"] = "Timik-47";
            map_weapons["AssaultRifle_07"] = "Stark AR";

            map_weapons["GrenadeLauncher_01"] = "Grenade Launcher";
            map_weapons["Grenade_01"] = "Grenade_01";
            map_weapons["Grenade_02"] = "Grenade_02";

            map_weapons["HeavyMachineGun_01"] = "Minigun";

            map_weapons["CricketBat_01"] = "Cricket Bat";
            map_weapons["Katana_01"] = "Katana";
            map_weapons["Knife_01"] = "Beckhill Combat Knife";
            map_weapons["Knife_02"] = "Stilnotto Stiletto";
            map_weapons["Knife_03"] = "Kukri";

            map_weapons["MachineGun_01"] = "MK46";
            map_weapons["MachineGun_02"] = "K-121";

            map_weapons["MachinePistol_01"] = "MP 400";
            map_weapons["MachinePistol_02"] = "Silenced MP 400";
            map_weapons["MachinePistol_03"] = "Tolen MP";
            map_weapons["MachinePistol_04"] = "Empire-9";

            map_weapons["MountedMG_01"] = "Mounted Turret";
            map_weapons["MountedMG_02"] = "MountedMG_02";

            map_weapons["Pistol_01"] = "M9";
            map_weapons["Pistol_02"] = "Silenced M9";
            map_weapons["Pistol_03"] = "DE .50";
            map_weapons["Pistol_04"] = "Simeon .357";
            map_weapons["Pistol_05"] = "Caulden";
            map_weapons["Pistol_06"] = "Selbstadt .40";
            map_weapons["Pistol_07"] = "Smjuth & Whetsman .40";

            map_weapons["ReviveGun_01"] = "Revive Gun";

            map_weapons["RocketLauncher_01"] = "Rocket Launcher";

            map_weapons["Shotgun_01"] = "Hollunds 880";
            map_weapons["Shotgun_02"] = "Remburg 7";
            map_weapons["Shotgun_03"] = "Ahnuhld-12";

            map_weapons["SniperRifle_01"] = "MOA SNPR-1";
            map_weapons["SniperRifle_02"] = "PDP-70";
            map_weapons["SniperRifle_03"] = "Fel-ix";
            map_weapons["SniperRifle_04"] = "Granduer SR";

            map_weapons["StickyBomb_01"] = "StickyBomb_01";
            map_weapons["StickyMine_01"] = "StickyMine_01";

            map_weapons["SubMachineGun_01"] = "SMG-9";
            map_weapons["SubMachineGun_02"] = "Hochfir";
            map_weapons["SubMachineGun_03"] = "KEK-10";
            map_weapons["SubMachineGun_04"] = "Crotzni";
            map_weapons["SubMachineGun_05"] = "Blishlok";

            map_weapons["Throwableknife_01"] = "Throwableknife_01";
            map_weapons["Tomahawk_01"] = "Tomahawk_01";
        }

        private void populateCombos()
        {
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
            {
                Characters.Items.Add(" ");
                Maps.Items.Add(" ");
                Primary.Items.Add(" ");
                Secondary.Items.Add(" ");
                Meele.Items.Add(" ");
                ItemCB.Items.Add(" ");
            }));
            foreach (KeyValuePair<string, string> ent in map_classes)
            {
                try
                {
                    Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
                    {
                        tConsole("Add to Characters Combobox: " + ent.Key);
                        Characters.Items.Add(ent.Key);
                    }));
                }
                catch (Exception ex)
                {
                    tConsole(ex.Message);
                    tConsole(ex.StackTrace);
                }
            }
            foreach (KeyValuePair<int, string> ent in maps)
            {
                try
                {
                        Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
                        {
                        
                            tConsole("Add Maps Combobox: " + ent.Value);
                            Maps.Items.Add(ent.Value);
                        }));
                }
                catch (Exception ex)
                {
                    tConsole(ex.Message);
                    tConsole(ex.StackTrace);
                }
            }
            if (!Properties.Settings.Default.quick_launch)
            {
                try
                {
                    var ordered = map_weapons.OrderBy(x => x.Value);
                    foreach (KeyValuePair<string, string> ent in ordered)
                    {
                        Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
                        {
                            tConsole("Add Weapons Comboboxes: " + ent.Value);
                            Primary.Items.Add(ent.Value);
                            Secondary.Items.Add(ent.Value);
                            Meele.Items.Add(ent.Value);
                        }));
                    }
                }
                catch (Exception ex)
                {
                    tConsole(ex.Message);
                    tConsole(ex.StackTrace);
                }
            }
            else
            {
                foreach (KeyValuePair<int, string> ent in weapons)
                {
                    try
                    {
                        Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
                        {
                            tConsole("Add Weapons Comboboxes: " + ent.Value);
                            Primary.Items.Add(ent.Value);
                            Secondary.Items.Add(ent.Value);
                            Meele.Items.Add(ent.Value);
                        }));
                    }
                    catch (Exception ex)
                    {
                        tConsole(ex.Message);
                        tConsole(ex.StackTrace);
                    }
                }
            }
            
            foreach (KeyValuePair<int, string> ent in items)
            {
                try
                {
                    Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
                    {
                        tConsole("Add Item Combobox: " + ent.Value);
                        ItemCB.Items.Add(ent.Value);
                    }));
                }
                catch (Exception ex)
                {
                    tConsole(ex.Message);
                    tConsole(ex.StackTrace);
                }
            }
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
            {
                Maps.Items.Add("ShooterEntry");
            }));
        }
        private void finished()
        {
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
            {
                commandBuilder.IsEnabled = true;
                commandBuilder.Text = "Select an option from the dropdowns to begin!";
                char_panel.IsEnabled = true;
                map_panel.IsEnabled = true;
                opt_panel.IsEnabled = true;
                Characters.SelectedIndex = 0;
                Maps.SelectedIndex = 0;
                Primary.SelectedIndex = 0;
                launching = false;
            }));
        }
        private void taskController()
        {
            cleanUp();
            enumDicts();
            enumMapDicts();
            populateCombos();
            finished();
            cleanUp();
        }
        private void cleanUp()
        {
            tConsole("Running Clean Up");
            if (Directory.Exists("temp")) Directory.Delete("temp", true);
            tConsole("Clean Up successful");
        }

        private void _commandConstructor(bool refresh = true)
        {
            char_panel.IsEnabled = false;
            map_panel.IsEnabled = false;
            opt_panel.IsEnabled = false;

            Dictionary<string, string> cmds = new Dictionary<string, string>();

            bool suicide = false;
            bool setChar = false;
            bool setPrimary = false;
            bool setSecondary = false;
            bool setMeele = false;
            bool setItem = false;

            if (refresh)
            {
                if ((bool)sui_com.IsChecked) suicide = true;
                if (Characters.SelectedIndex != 0 && Characters.SelectedIndex != -1) setChar = true;
                if (Primary.SelectedIndex != 0 && Primary.SelectedIndex != -1) setPrimary = true;
                if (Secondary.SelectedIndex != 0 && Secondary.SelectedIndex != -1) setSecondary = true;
                if (Meele.SelectedIndex != 0 && Meele.SelectedIndex != -1) setMeele = true;
                if (ItemCB.SelectedIndex != 0 && ItemCB.SelectedIndex != -1) setItem = true;

                if (setChar)
                {
                    string chrName = Characters.SelectedItem.ToString();
                    string chrFile = map_classes[chrName];
                    cmds.Add("charcmd", "set SGPlayerReplicationInfo m_SlotArcheTypes "+ chrFile +"_Gameplay.Pawns.A_"+chrFile);
                } else cmds.Add("charcmd", "noset");

                cmds.Add("primarycmd", setPrimary == true ? "set SGPawn PrimaryWeapons (A_" + Primary.SelectedItem + ")" : "noset");
                cmds.Add("secondarycmd", setSecondary == true ? "set SGPawn SecondaryWeapons (A_" + Secondary.SelectedItem + ")" : "noset");
                cmds.Add("meelecmd", setMeele == true ? "set SGPawn MeleeWeapons (A_" + Meele.SelectedItem + ")" : "noset");
                cmds.Add("itemcmd", setItem == true ? "set SGPawn Items (A_" + ItemCB.SelectedItem + ")" : "noset");

                //Build fullcommand
                string fc = "";
                //One-liner for maximum unreadability
                foreach (KeyValuePair<string, string> ent in cmds) if (ent.Value != "noset") if (fc != "") fc += " | " + ent.Value; else fc = ent.Value;
                if (suicide && fc != "") fc += " | Kill";
                cmds["fullcommand"] = fc;
                commandBuilder.Text = cmds["fullcommand"];
            }

            char_panel.IsEnabled = true;
            map_panel.IsEnabled = true;
            opt_panel.IsEnabled = true;

            commandBuilder.Focus();
            commandBuilder.SelectAll();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WriteResources();
            bool dbFilesFound = false;
            for (var i = 1; i < 4; i++)
            {
                if (findDBFiles() == false)
                {
                    tConsole("Trying again: " + i);
                }
                else
                {
                    dbFilesFound = true;
                    break;
                }
            }
            if (dbFilesFound)
            {
                Task task1 = Task.Factory.StartNew(() => taskController());
            }
            else
            {
                tConsole("The program was not successful in finding the Dirty Bomb files");
            }
            if (Properties.Settings.Default.quick_launch)
            {
                ql.IsChecked = true;
            }
            else
            {
                ql.IsChecked = false;
            }
            steampa.Text = Properties.Settings.Default.steam_path;
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!launching && !SCDialogOpen && ((ComboBox)sender).Name == "Maps")
            {
                SingleCommand diag = new SingleCommand();
                diag.setCommand("SwitchLevel " + Maps.SelectedItem);
                diag.ShowDialog();
                tConsole("Command Coppied");
                diag = null;
                SCDialogOpen = true;
                Maps.SelectedIndex = 0;
                SCDialogOpen = false;
            }
            else
            {
                _commandConstructor();
            }
        }

        private void SelectionChanged_Check(object sender, RoutedEventArgs e)
        {
            _commandConstructor();
        }

        private void ql_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)ql.IsChecked)
            {
                Properties.Settings.Default.quick_launch = true;
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.quick_launch = false;
                Properties.Settings.Default.Save();
            }
        }

        private void TextBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            chooseSteamFolder();
        }

        private void steampa_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox_MouseDown(null, null);
        }
        private void chooseSteamFolder()
        {
            try
            {
                var dialog = new CommonOpenFileDialog();
                dialog.IsFolderPicker = true;
                CommonFileDialogResult result = dialog.ShowDialog();
                Properties.Settings.Default.steam_path = dialog.FileNames.ElementAt(0);
                Properties.Settings.Default.Save();
                steampa.Text = Properties.Settings.Default.steam_path;
                tConsole("Selected folder: " + Properties.Settings.Default.steam_path);
            }
            catch (InvalidOperationException ex)
            {
                tConsole("Cancled choosing");
            }
            catch (Exception ex)
            {
                tConsole(ex.Message);
                tConsole(ex.StackTrace);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cleanUp();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SingleCommand diag = new SingleCommand();
            diag.setCommand("set SGPawn Health 9999999 | set SGPawn HealthMax 9999999 | set SGPlayerSetCooldown m_AbilityCoolDowns (m_autorate=10) | set SGCooldownComponent m_energycost (0)");
            diag.ShowDialog();
            tConsole("Command Coppied");
            diag = null;
        }
    }
}
