using Myra.Graphics2D.UI;
using System;

namespace FrozenPizza
{
    class Menu
    {
        public static void MainMenu(Engine engine, Desktop host)
        {
            host.Widgets.Clear();
            Grid grid = new Grid();

            grid.RowSpacing = 8;

            grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Pixels, 200));
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion());

            Button hostBtn = new Button();
            hostBtn.Text = "Host Game";
            hostBtn.GridRow = 0;
            hostBtn.HorizontalAlignment = HorizontalAlignment.Center;
            hostBtn.MouseDown += (s, a) =>
            {
                HostMenu(engine, host);
            };
            grid.Widgets.Add(hostBtn);

            Button playBtn = new Button();
            playBtn.Text = "Join Game";
            playBtn.GridRow = 1;
            playBtn.HorizontalAlignment = HorizontalAlignment.Center;
            playBtn.MouseDown += (s, a) =>
            {
                JoinMenu(engine, host);
            };
            grid.Widgets.Add(playBtn);

            Button optionsBtn = new Button();
            optionsBtn.Text = "Options";
            optionsBtn.GridRow = 2;
            optionsBtn.HorizontalAlignment = HorizontalAlignment.Center;
            optionsBtn.MouseDown += (s, a) =>
            {
                OptionsMenu(engine, host);
            };
            grid.Widgets.Add(optionsBtn);

            Button quitBtn = new Button();
            quitBtn.Text = "Quit";
            quitBtn.GridRow = 3;
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

            grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Pixels, 200));
            grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Pixels, 200));
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion());

            TextBlock nameLabel = new TextBlock();
            nameLabel.Text = "Server name";
            nameLabel.GridColumn = 0;
            nameLabel.GridRow = 0;
            nameLabel.HorizontalAlignment = HorizontalAlignment.Left;
            grid.Widgets.Add(nameLabel);

            TextField nameInput = new TextField();
            nameInput.Text = "FP Server";
            nameInput.GridColumn = 1;
            nameInput.GridRow = 0;
            nameInput.HorizontalAlignment = HorizontalAlignment.Right;
            grid.Widgets.Add(nameInput);

            TextBlock mapLabel = new TextBlock();
            mapLabel.Text = "Map";
            mapLabel.GridColumn = 0;
            mapLabel.GridRow = 1;
            mapLabel.HorizontalAlignment = HorizontalAlignment.Left;
            grid.Widgets.Add(mapLabel);

            ComboBox mapCombo = new ComboBox();
            mapCombo.GridColumn = 1;
            mapCombo.GridRow = 1;
            mapCombo.HorizontalAlignment = HorizontalAlignment.Right;
            grid.Widgets.Add(mapCombo);

            TextBlock slotsLabel = new TextBlock();
            slotsLabel.Text = "Slots";
            slotsLabel.GridColumn = 0;
            slotsLabel.GridRow = 2;
            slotsLabel.HorizontalAlignment = HorizontalAlignment.Left;
            grid.Widgets.Add(slotsLabel);

            HorizontalSlider slotsSlider = new HorizontalSlider();
            slotsSlider.GridColumn = 1;
            slotsSlider.GridRow = 2;
            slotsSlider.HorizontalAlignment = HorizontalAlignment.Right;
            grid.Widgets.Add(slotsSlider);

            Button hostBtn = new Button();
            hostBtn.Text = "Host";
            hostBtn.GridRow = 3;
            hostBtn.HorizontalAlignment = HorizontalAlignment.Center;
            hostBtn.MouseDown += (s, a) =>
            {
                //NetHandler.startServer();
            };
            grid.Widgets.Add(hostBtn);

            Button backBtn = new Button();
            backBtn.Text = "Back";
            backBtn.GridRow = 4;
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

            grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Pixels, 200));
            grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Pixels, 200));
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion());

            TextBlock serverLabel = new TextBlock();
            serverLabel.Text = "Server IP";
            serverLabel.GridColumn = 0;
            serverLabel.GridRow = 0;
            serverLabel.HorizontalAlignment = HorizontalAlignment.Left;
            grid.Widgets.Add(serverLabel);

            TextField serverInput = new TextField();
            serverInput.Text = "localhost";
            serverInput.GridColumn = 1;
            serverInput.GridRow = 0;
            serverInput.HorizontalAlignment = HorizontalAlignment.Right;
            grid.Widgets.Add(serverInput);

            Button joinBtn = new Button();
            joinBtn.Text = "Join";
            joinBtn.GridRow = 1;
            joinBtn.HorizontalAlignment = HorizontalAlignment.Center;
            joinBtn.MouseDown += (s, a) =>
            {
                /*Engine.netHandle = new NetHandler();
                int port;

                if (_ip.Length > 0 && Int32.TryParse(_port, out port))
                {
                    _connecting = true;
                    Engine.netHandle.connect(_ip, port);
                }
                
                
                if (Engine.netHandle.Handshake && !Engine.netHandle.GameReady)
                {
                    _engine.InitializeGame();
                    _engine.LoadGame();
                    Engine.netHandle.GameReady = true;
                }
                else if (Engine.netHandle.Ready)
                {
                    _engine.toggleMouseVisible();
                    _engine.gstate = GameState.Playing;
                    _engine.setMenu(new GameMenu(_engine));
                }
                 */
            };
            grid.Widgets.Add(joinBtn);

            Button backBtn = new Button();
            backBtn.Text = "Back";
            backBtn.GridRow = 2;
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

            grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Pixels, 200));
            grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Pixels, 200));
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion());
            grid.RowsProportions.Add(new Grid.Proportion());

            TextBlock playerLabel = new TextBlock();
            playerLabel.Text = "Player name";
            playerLabel.GridColumn = 0;
            playerLabel.GridRow = 0;
            playerLabel.HorizontalAlignment = HorizontalAlignment.Left;
            grid.Widgets.Add(playerLabel);

            TextField playerInput = new TextField();
            playerInput.Text = "player";
            playerInput.GridColumn = 1;
            playerInput.GridRow = 0;
            playerInput.HorizontalAlignment = HorizontalAlignment.Right;
            grid.Widgets.Add(playerInput);

            TextBlock resolutionLabel = new TextBlock();
            resolutionLabel.Text = "Resolution";
            resolutionLabel.GridColumn = 0;
            resolutionLabel.GridRow = 1;
            resolutionLabel.HorizontalAlignment = HorizontalAlignment.Left;
            grid.Widgets.Add(resolutionLabel);

            ComboBox resolutionCombo = new ComboBox();
            resolutionCombo.GridColumn = 1;
            resolutionCombo.GridRow = 1;
            resolutionCombo.HorizontalAlignment = HorizontalAlignment.Right;
            grid.Widgets.Add(resolutionCombo);

            TextBlock musicLabel = new TextBlock();
            musicLabel.Text = "Music volume";
            musicLabel.GridColumn = 0;
            musicLabel.GridRow = 2;
            musicLabel.HorizontalAlignment = HorizontalAlignment.Left;
            grid.Widgets.Add(musicLabel);

            HorizontalSlider musicSlider = new HorizontalSlider();
            musicSlider.GridColumn = 1;
            musicSlider.GridRow = 2;
            musicSlider.HorizontalAlignment = HorizontalAlignment.Right;
            grid.Widgets.Add(musicSlider);

            TextBlock soundLabel = new TextBlock();
            soundLabel.Text = "Sounds volume";
            soundLabel.GridColumn = 0;
            soundLabel.GridRow = 3;
            soundLabel.HorizontalAlignment = HorizontalAlignment.Left;
            grid.Widgets.Add(soundLabel);

            HorizontalSlider soundSlider = new HorizontalSlider();
            soundSlider.GridColumn = 1;
            soundSlider.GridRow = 3;
            soundSlider.HorizontalAlignment = HorizontalAlignment.Right;
            grid.Widgets.Add(soundSlider);

            Button applyBtn = new Button();
            applyBtn.Text = "Apply";
            applyBtn.GridRow = 4;
            applyBtn.HorizontalAlignment = HorizontalAlignment.Center;
            applyBtn.MouseDown += (s, a) =>
            {
                //Options.Config.Fullscreen = _settingSelectedValue[0] == 1 ? true : false;
                //Options.Config.Width = Options.getDisplayModeById(_settingSelectedValue[1]).Width;
                //Options.Config.Height = Options.getDisplayModeById(_settingSelectedValue[1]).Height;
                //Options.Config.MusicVolume = 0.25f * _settingSelectedValue[2];
                //Options.Config.SoundVolume = 0.25f * _settingSelectedValue[2];
                Options.applyConfig();
                Options.SetConfigFile();
            };
            grid.Widgets.Add(applyBtn);

            Button backBtn = new Button();
            backBtn.Text = "Back";
            backBtn.GridRow = 5;
            backBtn.HorizontalAlignment = HorizontalAlignment.Center;
            backBtn.MouseDown += (s, a) =>
            {
                MainMenu(engine, host);
            };
            grid.Widgets.Add(backBtn);

            host.Widgets.Add(grid);
        }
    }
}
