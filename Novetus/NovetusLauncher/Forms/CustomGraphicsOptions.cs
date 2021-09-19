﻿#region Usings
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Linq;
#endregion

namespace NovetusLauncher
{
    #region CustomGraphicsOptions
    public partial class CustomGraphicsOptions : Form
    {
        #region Private Variables
        private int QualityLevel = 0;
        private int MeshDetail = 100;
        private int ShadingQuality = 100;
        private int MaterialQuality = 0;
        private int AA = 0;
        private int AASamples = 1;
        private int Bevels = 0;
        private int Shadows_2008 = 0;
        private bool Shadows_2007 = false;
        private string Style_2007 = "";
        private string WindowResolution = "";
        private string FullscreenResolution = "";
        private bool initial = true;
        private int ModernResolution = 0;
        private FileFormat.ClientInfo info;
        #endregion

        #region Constructor
        public CustomGraphicsOptions()
        {
            InitializeComponent();
        }
        #endregion

        #region Form Events
        private void CustomGraphicsOptions_Load(object sender, EventArgs e)
        {
            GlobalFuncs.ReadClientValues(GlobalVars.UserConfiguration.SelectedClient, null);
            info = GlobalFuncs.GetClientInfoValues(GlobalVars.UserConfiguration.SelectedClient);

            string terms = "_" + GlobalVars.UserConfiguration.SelectedClient;
            bool hasFoundDir = false;
            string[] dirs = Directory.GetFiles(GlobalPaths.ConfigDirClients);

            foreach (string dir in dirs)
            {
                if (dir.Contains(terms) && !dir.Contains("_default"))
                {
                    string oldfile = "";
                    string fixedfile = "";
                    XDocument doc = null;

                    try
                    {
                        oldfile = File.ReadAllText(dir);
                        fixedfile = RobloxXML.RemoveInvalidXmlChars(RobloxXML.ReplaceHexadecimalSymbols(oldfile));
                        doc = XDocument.Parse(fixedfile);
                    }
                    catch (Exception ex)
                    {
                        GlobalFuncs.LogExceptions(ex);
                        return;
                    }

                    //read a few handpicked things to not take up memory

                    try
                    {
                        MeshDetail = Convert.ToInt32(RobloxXML.GetRenderSettings(doc, "maxMeshDetail", XMLTypes.Float));
                        GraphicsMeshQuality.Value = MeshDetail;
                    }
                    catch (Exception ex)
                    {
                        GlobalFuncs.LogExceptions(ex);
                        GraphicsMeshQuality.Enabled = false;
                    }

                    try
                    {
                        ShadingQuality = Convert.ToInt32(RobloxXML.GetRenderSettings(doc, "maxShadingQuality", XMLTypes.Float));
                        GraphicsShadingQuality.Value = ShadingQuality;
                    }
                    catch (Exception ex)
                    {
                        GlobalFuncs.LogExceptions(ex);
                        GraphicsShadingQuality.Enabled = false;
                    }

                    try
                    {
                        MaterialQuality = Convert.ToInt32(RobloxXML.GetRenderSettings(doc, "WoodQuality", XMLTypes.Token));
                        GraphicsMaterialQuality.SelectedIndex = MaterialQuality;
                    }
                    catch (Exception ex)
                    {
                        GlobalFuncs.LogExceptions(ex);
                        try
                        {
                            MaterialQuality = Convert.ToInt32(RobloxXML.GetRenderSettings(doc, "TrussDetail", XMLTypes.Token));
                            GraphicsMaterialQuality.SelectedIndex = MaterialQuality;
                        }
                        catch (Exception ex2)
                        {
                            GlobalFuncs.LogExceptions(ex2);
                            GraphicsMaterialQuality.Enabled = false;
                        }
                    }

                    try
                    {
                        AA = Convert.ToInt32(RobloxXML.GetRenderSettings(doc, "Antialiasing", XMLTypes.Token));
                        GraphicsAntiAliasing.SelectedIndex = AA;
                    }
                    catch (Exception ex)
                    {
                        GlobalFuncs.LogExceptions(ex);
                        GraphicsAntiAliasing.Enabled = false;
                    }

                    try
                    {
                        AASamples = Convert.ToInt32(RobloxXML.GetRenderSettings(doc, "AASamples", XMLTypes.Token));

                        switch (AASamples)
                        {
                            case 4:
                                GraphicsAASamples.SelectedIndex = 1;
                                break;
                            case 8:
                                GraphicsAASamples.SelectedIndex = 2;
                                break;
                            default:
                                GraphicsAASamples.SelectedIndex = 0;
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobalFuncs.LogExceptions(ex);
                        GraphicsAASamples.Enabled = false;
                    }

                    try
                    {
                        Bevels = Convert.ToInt32(RobloxXML.GetRenderSettings(doc, "Bevels", XMLTypes.Token));
                        GraphicsBevels.SelectedIndex = Bevels;
                    }
                    catch (Exception ex)
                    {
                        GlobalFuncs.LogExceptions(ex);
                        GraphicsBevels.Enabled = false;
                    }

                    try
                    {
                        Shadows_2008 = Convert.ToInt32(RobloxXML.GetRenderSettings(doc, "Shadow", XMLTypes.Token));
                        GraphicsShadows2008.SelectedIndex = Shadows_2008;
                    }
                    catch (Exception ex)
                    {
                        GlobalFuncs.LogExceptions(ex);
                        GraphicsShadows2008.Enabled = false;
                    }

                    try
                    {
                        Shadows_2007 = Convert.ToBoolean(RobloxXML.GetRenderSettings(doc, "Shadows", XMLTypes.Bool));
                    }
                    catch (Exception ex)
                    {
                        GlobalFuncs.LogExceptions(ex);
                        // try doing march 2007.
                        try
                        {
                            Shadows_2007 = Convert.ToBoolean(RobloxXML.GetRenderSettings(doc, "shadows", XMLTypes.Bool));
                        }
                        catch (Exception ex2)
                        {
                            GlobalFuncs.LogExceptions(ex2);
                            GraphicsShadows2007.Enabled = false;
                        }
                    }

                    if (GraphicsShadows2007.Enabled)
                    {
                        switch (Shadows_2007)
                        {
                            case false:
                                GraphicsShadows2007.SelectedIndex = 1;
                                break;
                            default:
                                GraphicsShadows2007.SelectedIndex = 0;
                                break;
                        }
                    }

                    try
                    {
                        bool checkSkin = RobloxXML.IsRenderSettingStringValid(doc, "_skinFile", XMLTypes.String);
                        if (checkSkin)
                        {
                            Style_2007 = RobloxXML.GetRenderSettings(doc, "_skinFile", XMLTypes.String).Replace(@"Styles\", "");
                            Style2007.Text = Style_2007;
                        }
                        else
                        {
                            Style2007.Enabled = false;
                            Style2007FolderFinder.Enabled = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobalFuncs.LogExceptions(ex);
                        Style2007.Enabled = false;
                        Style2007FolderFinder.Enabled = false;
                    }

                    try
                    {
                        QualityLevel = Convert.ToInt32(RobloxXML.GetRenderSettings(doc, "QualityLevel", XMLTypes.Token));
                        GraphicsLevel.Value = QualityLevel;
                    }
                    catch (Exception ex)
                    {
                        GlobalFuncs.LogExceptions(ex);
                        GraphicsLevel.Enabled = false;
                    }

                    try
                    {
                        FullscreenResolution = RobloxXML.GetRenderSettings(doc, "FullscreenSize", XMLTypes.Vector2Int16);

                        if (!string.IsNullOrWhiteSpace(FullscreenResolution))
                        {
                            GraphicsFullscreenResolution.Text = FullscreenResolution;
                        }
                        else
                        {
                            FullscreenResolution = RobloxXML.GetRenderSettings(doc, "FullscreenSizePreference", XMLTypes.Vector2Int16);
                            if (!string.IsNullOrWhiteSpace(FullscreenResolution))
                            {
                                GraphicsFullscreenResolution.Text = FullscreenResolution;
                            }
                            else
                            {
                                GraphicsFullscreenResolution.Enabled = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobalFuncs.LogExceptions(ex);
                        GraphicsFullscreenResolution.Enabled = false;
                    }

                    try
                    {
                        WindowResolution = RobloxXML.GetRenderSettings(doc, "WindowSize", XMLTypes.Vector2Int16);

                        if (!string.IsNullOrWhiteSpace(WindowResolution))
                        {
                            GraphicsWindowResolution.Text = WindowResolution;
                        }
                        else
                        {
                            WindowResolution = RobloxXML.GetRenderSettings(doc, "WindowSizePreference", XMLTypes.Vector2Int16);
                            if (!string.IsNullOrWhiteSpace(WindowResolution))
                            {
                                GraphicsWindowResolution.Text = WindowResolution;
                            }
                            else
                            {
                                GraphicsWindowResolution.Enabled = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobalFuncs.LogExceptions(ex);
                        GraphicsWindowResolution.Enabled = false;
                    }

                    try
                    {
                        ModernResolution = Convert.ToInt32(RobloxXML.GetRenderSettings(doc, "Resolution", XMLTypes.Token));
                        GraphicsModernResolution.SelectedIndex = ModernResolution;
                    }
                    catch (Exception ex)
                    {
                        GlobalFuncs.LogExceptions(ex);
                        GraphicsModernResolution.Enabled = false;
                    }

                    doc = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    hasFoundDir = true;
                }
            }

            if (!hasFoundDir)
            {
                MessageBox.Show("This client does not support setting adjustment through the Novetus Launcher.\nTry opening this client in ROBLOX Studio and adjust it through the settings in Tools -> Settings.", "Novetus - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }

            initial = false;
        }

        private void GraphicsLevel_ValueChanged(object sender, EventArgs e)
        {
            if (GraphicsLevel.Value > 19 && (info.ClientLoadOptions != Settings.ClientLoadOptions.Client_2008AndUp_ForceAutomaticQL21 ||
                info.ClientLoadOptions != Settings.ClientLoadOptions.Client_2008AndUp_QualityLevel21))
            {
                GraphicsLevel.Value = 19;
            }

            QualityLevel = Convert.ToInt32(GraphicsLevel.Value);
        }

        private void GraphicsLevel_Click(object sender, EventArgs e)
        {
            if (GraphicsLevel.Value > 19 && (info.ClientLoadOptions != Settings.ClientLoadOptions.Client_2008AndUp_ForceAutomaticQL21 ||
                info.ClientLoadOptions != Settings.ClientLoadOptions.Client_2008AndUp_QualityLevel21))
            {
                MessageBox.Show("This client does not support quality levels above 19.", "Novetus - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GraphicsMeshQuality_ValueChanged(object sender, EventArgs e)
        {
            MeshDetail = Convert.ToInt32(GraphicsMeshQuality.Value);
        }

        private void GraphicsShadingQuality_ValueChanged(object sender, EventArgs e)
        {
            ShadingQuality = Convert.ToInt32(GraphicsShadingQuality.Value);
        }

        private void GraphicsMaterialQuality_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!GraphicsMaterialQuality.Enabled)
            {
                GraphicsAntiAliasing.SelectedIndex = 0;
            }

            MaterialQuality = GraphicsMaterialQuality.SelectedIndex;
        }

        private void GraphicsAntiAliasing_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!GraphicsAntiAliasing.Enabled)
            {
                GraphicsAntiAliasing.SelectedIndex = 0;
            }

            AA = GraphicsAntiAliasing.SelectedIndex;
        }

        private void GraphicsAASamples_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (GraphicsAASamples.SelectedIndex)
            {
                case 1:
                    AASamples = 4;
                    break;
                case 2:
                    AASamples = 8;
                    break;
                default:
                    AASamples = 1;
                    break;
            }
        }

        private void GraphicsBevels_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!GraphicsBevels.Enabled)
            {
                GraphicsBevels.SelectedIndex = 0;
            }

            Bevels = GraphicsBevels.SelectedIndex;
        }

        private void GraphicsShadows2008_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!GraphicsShadows2008.Enabled)
            {
                GraphicsShadows2008.SelectedIndex = 0;
            }

            Shadows_2008 = GraphicsShadows2008.SelectedIndex;
        }

        private void Style2007_TextChanged(object sender, EventArgs e)
        {
            Style_2007 = Style2007.Text;
        }

        private void GraphicsWindowResolution_TextChanged(object sender, EventArgs e)
        {
            if (!initial && !CheckIfResolutionString(GraphicsWindowResolution.Text))
            {
                MessageBox.Show("Please input a valid resolution. i.e 800x600 or 1024x768. Novetus will reset back to the default resolution.", "Novetus - Error when inputting resolution.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                GraphicsWindowResolution.Text = "800x600";
            }

            WindowResolution = GraphicsWindowResolution.Text;
        }

        private void GraphicsFullscreenResolution_TextChanged(object sender, EventArgs e)
        {
            if (!initial && !CheckIfResolutionString(GraphicsFullscreenResolution.Text))
            {
                MessageBox.Show("Please input a valid resolution. i.e 800x600 or 1024x768. Novetus will reset back to the default resolution.", "Novetus - Error when inputting resolution.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                GraphicsFullscreenResolution.Text = "1024x768";
            }

            FullscreenResolution = GraphicsFullscreenResolution.Text;
        }

        private void GraphicsModernResolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            ModernResolution = GraphicsModernResolution.SelectedIndex;
        }

        private void Style2007FolderFinder_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", GlobalPaths.ClientDir.Replace(@"\\", @"\") + @"\" + GlobalVars.UserConfiguration.SelectedClient + @"\Styles");
        }

        private void Styles2007Info_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Make sure you place the styles you want in the Styles folder in " + GlobalPaths.ClientDir.Replace(@"\\", @"\") + @"\" + GlobalVars.UserConfiguration.SelectedClient + @"\Styles." + Environment.NewLine + "If the files are not placed in this directory, they will not be loaded properly.\nThis client will accept .msstyles and .cjstyles files.", "Novetus - Styles Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void GraphicsShadows2007_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (GraphicsShadows2007.SelectedIndex)
            {
                case 1:
                    Shadows_2007 = false;
                    break;
                default:
                    Shadows_2007 = true;
                    break;
            }
        }

        private void CustomGraphicsOptions_Close(object sender, FormClosingEventArgs e)
        {
            GlobalFuncs.ReadClientValues(GlobalVars.UserConfiguration.SelectedClient, null);
            GlobalFuncs.ApplyClientSettings_custom(info, GlobalVars.UserConfiguration.SelectedClient, MeshDetail, ShadingQuality, MaterialQuality,
                        AA, AASamples, Bevels, Shadows_2008, Shadows_2007, Style_2007, QualityLevel, 
                        WindowResolution, FullscreenResolution, ModernResolution);
        }
        #endregion

        #region Functions
        private bool CheckIfResolutionString(string resString)
        {
            try
            {
                string[] resStringTest = resString.Split('x');

                if (resStringTest.Length > 0)
                {
                    if (int.TryParse(resStringTest[0], out _))
                    {
                        if (int.TryParse(resStringTest[1], out _))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                GlobalFuncs.LogExceptions(ex);
                return false;
            }
        }
        #endregion
    }
    #endregion
}
