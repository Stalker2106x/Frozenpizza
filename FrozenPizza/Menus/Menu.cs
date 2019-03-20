using Microsoft.Xna.Framework.Content;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using System;
using System.Collections.Generic;
using System.Threading;

namespace FrozenPizza
{
    class Menu
    {
        public static void Init(ContentManager content)
        {
            /*SpriteFont font = content.Load<SpriteFont>("font/general");
            Stylesheet.Current.TextBlockStyle.Font = font;*/
            Stylesheet.Current.ButtonStyle.Width = 100;
            Stylesheet.Current.ComboBoxStyle.Width = 100;
            Stylesheet.Current.TextFieldStyle.Width = 100;
        }

        public static void MainMenu(Engine engine, Desktop host)
        {
            host.Widgets.Clear();
            Grid grid = new Grid();

            grid.RowSpacing = 8;

            grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Part));
            grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Pixels, 200));
            grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Part));
            grid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Part));
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Part));

            Button hostBtn = new Button();
            hostBtn.Text = "Host Game";
            hostBtn.GridColumn = 1;
            hostBtn.GridRow = 1;
            hostBtn.HorizontalAlignment = HorizontalAlignment.Center;
            hostBtn.MouseDown += (s, a) =>
            {
                HostMenu(engine, host);
            };
            grid.Widgets.Add(hostBtn);

            Button playBtn = new Button();
            playBtn.Text = "Join Game";
            playBtn.GridColumn = 1;
            playBtn.GridRow = 2;
            playBtn.HorizontalAlignment = HorizontalAlignment.Center;
            playBtn.MouseDown += (s, a) =>
            {
                JoinMenu(engine, host);
            };
            grid.Widgets.Add(playBtn);

            Button optionsBtn = new Button();
            optionsBtn.Text = "Options";
            optionsBtn.GridColumn = 1;
            optionsBtn.GridRow = 3;
            optionsBtn.HorizontalAlignment = HorizontalAlignment.Center;
            optionsBtn.MouseDown += (s, a) =>
            {
                OptionsMenu(engine, host);
            };
            grid.Widgets.Add(optionsBtn);

            Button quitBtn = new Button();
            quitBtn.Text = "Quit";
            quitBtn.GridColumn = 1;
            quitBtn.GridRow = 4;
            quitBtn.HorizontalAlignment = HorizontalAlignment.Center;
            quitBtn.MouseDown += (s, a) =>
            {
                engine.exit();
            };
            grid.Widgets.Add(quitBtn);

            host.Widgets.Add(grid);
        }

        public static void HostMenu(Engine engine, Desktop host)
        {
            host.Widgets.Clear();
            Grid grid = new Grid();
            grid.RowSpacing = 8;

            grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Part));
            grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Pixels, 200));
            grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Pixels, 200));
            grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Part));
            grid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Part));
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Part));

            TextBlock nameLabel = new TextBlock();
            nameLabel.Text = "Server name";
            nameLabel.GridColumn = 1;
            nameLabel.GridRow = 1;
            nameLabel.HorizontalAlignment = HorizontalAlignment.Left;
            grid.Widgets.Add(nameLabel);

            TextField nameInput = new TextField();
            nameInput.Text = "FP Server";
            nameInput.GridColumn = 2;
            nameInput.GridRow = 1;
            nameInput.HorizontalAlignment = HorizontalAlignment.Right;
            grid.Widgets.Add(nameInput);

            TextBlock mapLabel = new TextBlock();
            mapLabel.Text = "Map";
            mapLabel.GridColumn = 1;
            mapLabel.GridRow = 2;
            mapLabel.HorizontalAlignment = HorizontalAlignment.Left;
            grid.Widgets.Add(mapLabel);

            ComboBox mapCombo = new ComboBox();
            mapCombo.GridColumn = 2;
            mapCombo.GridRow = 2;
            mapCombo.HorizontalAlignment = HorizontalAlignment.Right;
            List<string> maps = Level.getAvailableLevels();
            maps.ForEach(e => { mapCombo.Items.Add(new ListItem(e)); });
            grid.Widgets.Add(mapCombo);

            TextBlock slotsLabel = new TextBlock();
            slotsLabel.Text = "Slots";
            slotsLabel.GridColumn = 1;
            slotsLabel.GridRow = 3;
            slotsLabel.HorizontalAlignment = HorizontalAlignment.Left;
            grid.Widgets.Add(slotsLabel);

            SpinButton slotsSlider = new SpinButton();
            slotsSlider.GridColumn = 2;
            slotsSlider.GridRow = 3;
            slotsSlider.HorizontalAlignment = HorizontalAlignment.Right;
            grid.Widgets.Add(slotsSlider);

            Button hostBtn = new Button();
            hostBtn.Text = "Host";
            hostBtn.GridColumn = 1;
            hostBtn.GridRow = 4;
            hostBtn.GridColumnSpan = 2;
            hostBtn.HorizontalAlignment = HorizontalAlignment.Center;
            hostBtn.MouseDown += (s, a) =>
            {
                if (mapCombo.SelectedItem == null)
                {
                    var messageBox = Dialog.CreateMessageBox("Error", "You must select a map!");
                    messageBox.ShowModal(host);
                    return;
                }
                NetHandler.startServer(mapCombo.SelectedItem.Text);
            };
            grid.Widgets.Add(hostBtn);

            Button backBtn = new Button();
            backBtn.Text = "Back";
            backBtn.GridColumn = 1;
            backBtn.GridRow = 5;
            backBtn.GridColumnSpan = 2;
            backBtn.HorizontalAlignment = HorizontalAlignment.Center;
            backBtn.MouseDown += (s, a) =>
            {
                MainMenu(engine, host);
            };
            grid.Widgets.Add(backBtn);

            host.Widgets.Add(grid);
        }

        public static void JoinMenu(Engine engine, Desktop host)
        {
            host.Widgets.Clear();
            Grid grid = new Grid();

            grid.RowSpacing = 8;

            grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Part));
            grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Pixels, 200));
            grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Pixels, 200));
            grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Part));
            grid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Part));
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Part));

            TextBlock serverLabel = new TextBlock();
            serverLabel.Text = "Server IP";
            serverLabel.GridColumn = 1;
            serverLabel.GridRow = 1;
            serverLabel.HorizontalAlignment = HorizontalAlignment.Left;
            grid.Widgets.Add(serverLabel);

            TextField serverInput = new TextField();
            serverInput.Text = "localhost";
            serverInput.GridColumn = 2;
            serverInput.GridRow = 1;
            serverInput.HorizontalAlignment = HorizontalAlignment.Right;
            grid.Widgets.Add(serverInput);

            Button joinBtn = new Button();
            joinBtn.Text = "Join";
            joinBtn.GridColumn = 1;
            joinBtn.GridRow = 2;
            joinBtn.GridColumnSpan = 2;
            joinBtn.HorizontalAlignment = HorizontalAlignment.Center;
            joinBtn.MouseDown += (s, a) =>
            {
                Engine.netHandle = new NetHandler();
                string server = serverInput.Text;
                int port;

                if (server.Length <= 0)
                {
                    Dialog messageBox = Dialog.CreateMessageBox("Error", "Bad server provided!");
                    messageBox.ShowModal(host);
                    return;
                }
                int pos = server.IndexOf(":");
                if (pos != -1) Int32.TryParse(server.Substring(pos, server.Length - pos), out port);
                else port = 27420;
                Engine.netHandle.connect(server, port);
                TimeSpan connectionTimeout = new TimeSpan();
                while (connectionTimeout.Seconds < 10) //10s timeout
                {
                    if (Engine.netHandle.Handshake && !Engine.netHandle.GameReady)
                    {
                        engine.InitializeGame();
                        engine.LoadGame();
                        Engine.netHandle.GameReady = true;
                    }
                    else if (Engine.netHandle.Ready)
                    {
                        engine.toggleMouseVisible();
                        engine.gstate = GameState.Playing;
                        GameMenu(engine, host);
                        return;
                    }
                    Thread.Sleep(1000);
                    connectionTimeout = connectionTimeout.Add(TimeSpan.FromSeconds(1));
                }
                Dialog errorBox = Dialog.CreateMessageBox("Error", "Could not reach server");
                errorBox.ShowModal(host);
                return;
            };
            grid.Widgets.Add(joinBtn);

            Button backBtn = new Button();
            backBtn.Text = "Back";
            backBtn.GridColumn = 1;
            backBtn.GridRow = 3;
            backBtn.GridColumnSpan = 2;
            backBtn.HorizontalAlignment = HorizontalAlignment.Center;
            backBtn.MouseDown += (s, a) =>
            {
                MainMenu(engine, host);
            };
            grid.Widgets.Add(backBtn);

            host.Widgets.Add(grid);
        }

        public static void OptionsMenu(Engine engine, Desktop host)
        {
            host.Widgets.Clear();
            Grid grid = new Grid();

            grid.RowSpacing = 8;

            grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Part));
            grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Pixels, 200));
            grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Pixels, 200));
            grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Part));
            grid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Part));
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Part));

            TextBlock playerLabel = new TextBlock();
            playerLabel.Text = "Player name";
            playerLabel.GridColumn = 1;
            playerLabel.GridRow = 1;
            playerLabel.HorizontalAlignment = HorizontalAlignment.Left;
            grid.Widgets.Add(playerLabel);

            TextField playerInput = new TextField();
            playerInput.Text = "player";
            playerInput.GridColumn = 2;
            playerInput.GridRow = 1;
            playerInput.HorizontalAlignment = HorizontalAlignment.Right;
            grid.Widgets.Add(playerInput);

            TextBlock resolutionLabel = new TextBlock();
            resolutionLabel.Text = "Resolution";
            resolutionLabel.GridColumn = 1;
            resolutionLabel.GridRow = 2;
            resolutionLabel.HorizontalAlignment = HorizontalAlignment.Left;
            grid.Widgets.Add(resolutionLabel);

            ComboBox resolutionCombo = new ComboBox();
            resolutionCombo.GridColumn = 2;
            resolutionCombo.GridRow = 2;
            resolutionCombo.HorizontalAlignment = HorizontalAlignment.Right;
            List<string> resolutions = Options.getResolutions();
            resolutions.ForEach(e => { resolutionCombo.Items.Add(new ListItem(e)); });
            resolutionCombo.SelectedIndex = Options.getDisplayMode();
            grid.Widgets.Add(resolutionCombo);

            TextBlock displayLabel = new TextBlock();
            displayLabel.Text = "Display mode";
            displayLabel.GridColumn = 1;
            displayLabel.GridRow = 3;
            displayLabel.HorizontalAlignment = HorizontalAlignment.Left;
            grid.Widgets.Add(displayLabel);

            ComboBox displayCombo = new ComboBox();
            displayCombo.GridColumn = 2;
            displayCombo.GridRow = 3;
            displayCombo.HorizontalAlignment = HorizontalAlignment.Right;
            displayCombo.Items.Add(new ListItem("Windowed"));
            displayCombo.Items.Add(new ListItem("Fullscreen"));
            displayCombo.SelectedIndex = Convert.ToInt32(Options.Config.Fullscreen);
            grid.Widgets.Add(displayCombo);

            TextBlock musicLabel = new TextBlock();
            musicLabel.Text = "Music volume";
            musicLabel.GridColumn = 1;
            musicLabel.GridRow = 4;
            musicLabel.HorizontalAlignment = HorizontalAlignment.Left;
            grid.Widgets.Add(musicLabel);

            HorizontalSlider musicSlider = new HorizontalSlider();
            musicSlider.GridColumn = 2;
            musicSlider.GridRow = 4;
            musicSlider.Width = 100;
            musicSlider.HorizontalAlignment = HorizontalAlignment.Right;
            musicSlider.Minimum = 0f;
            musicSlider.Maximum = 1f;
            grid.Widgets.Add(musicSlider);

            TextBlock soundLabel = new TextBlock();
            soundLabel.Text = "Sounds volume";
            soundLabel.GridColumn = 1;
            soundLabel.GridRow = 5;
            soundLabel.HorizontalAlignment = HorizontalAlignment.Left;
            grid.Widgets.Add(soundLabel);

            HorizontalSlider soundSlider = new HorizontalSlider();
            soundSlider.GridColumn = 2;
            soundSlider.GridRow = 5;
            soundSlider.Width = 100;
            soundSlider.HorizontalAlignment = HorizontalAlignment.Right;
            soundSlider.Minimum = 0f;
            soundSlider.Maximum = 1f;
            grid.Widgets.Add(soundSlider);

            Button applyBtn = new Button();
            applyBtn.Text = "Apply";
            applyBtn.GridColumn = 1;
            applyBtn.GridRow = 6;
            applyBtn.GridColumnSpan = 2;
            applyBtn.HorizontalAlignment = HorizontalAlignment.Center;
            applyBtn.MouseDown += (s, a) =>
            {
                if (displayCombo.SelectedItem == null || resolutionCombo.SelectedItem == null)
                {
                    var messageBox = Dialog.CreateMessageBox("Error", "You must select a value for display/resolution!");
                    messageBox.ShowModal(host);
                    return;
                }
                Options.Config.Fullscreen = Convert.ToBoolean(displayCombo.SelectedIndex);
                Options.Config.Width = Options.Resolutions[(int)resolutionCombo.SelectedIndex].Width;
                Options.Config.Height = Options.Resolutions[(int)resolutionCombo.SelectedIndex].Height;
                Options.Config.MusicVolume = musicSlider.Value;
                Options.Config.SoundVolume = soundSlider.Value;
                Options.applyConfig();
                engine.InitializeGraphics();
                Options.SetConfigFile();
                OptionsMenu(engine, host);
            };
            grid.Widgets.Add(applyBtn);

            Button backBtn = new Button();
            backBtn.Text = "Back";
            backBtn.GridColumn = 1;
            backBtn.GridRow = 7;
            backBtn.GridColumnSpan = 2;
            backBtn.HorizontalAlignment = HorizontalAlignment.Center;
            backBtn.MouseDown += (s, a) =>
            {
                MainMenu(engine, host);
            };
            grid.Widgets.Add(backBtn);

            host.Widgets.Add(grid);
        }

        public static void GameMenu(Engine engine, Desktop host)
        {
            host.Widgets.Clear();
            Grid grid = new Grid();

            grid.RowSpacing = 8;

            grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Part));
            grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Pixels, 200));
            grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Part));
            grid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Part));
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Part));

            Button resumeBtn = new Button();
            resumeBtn.Text = "Resume";
            resumeBtn.GridColumn = 1;
            resumeBtn.GridRow = 1;
            resumeBtn.HorizontalAlignment = HorizontalAlignment.Center;
            resumeBtn.MouseDown += (s, a) =>
            {
                engine.gstate = GameState.Playing;
            };
            grid.Widgets.Add(resumeBtn);

            Button disconnectBtn = new Button();
            disconnectBtn.Text = "Disconnect";
            disconnectBtn.GridColumn = 1;
            disconnectBtn.GridRow = 2;
            disconnectBtn.HorizontalAlignment = HorizontalAlignment.Center;
            disconnectBtn.MouseDown += (s, a) =>
            {
                engine.UnloadGame();
                if (NetHandler.Connected)
                {
                    NetHandler.disconnect();
                    Engine.netHandle = null;
                }
                MainMenu(engine, host);
            };
            grid.Widgets.Add(disconnectBtn);

            Button optionsBtn = new Button();
            optionsBtn.Text = "Options";
            optionsBtn.GridColumn = 1;
            optionsBtn.GridRow = 3;
            optionsBtn.HorizontalAlignment = HorizontalAlignment.Center;
            optionsBtn.MouseDown += (s, a) =>
            {
                MainMenu(engine, host);
            };
            grid.Widgets.Add(optionsBtn);

            host.Widgets.Add(grid);
        }
    }
}
