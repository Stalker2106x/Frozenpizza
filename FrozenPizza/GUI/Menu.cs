using FrozenPizza.Settings;
using Microsoft.Xna.Framework.Content;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using System;
using System.Collections.Generic;
using System.Threading;

namespace FrozenPizza
{
  public class Menu
  {
    public static void Init(ContentManager content)
    {
      /*SpriteFont font = content.Load<SpriteFont>("font/general");
      Stylesheet.Current.LabelStyle.Font = font;*/
      //Stylesheet.Current.TextButtonStyle.Width = 100;
      //Stylesheet.Current.ComboBoxStyle.Width = 100;
      //Stylesheet.Current.TextBoxStyle.Width = 100;
    }

    public static void MainMenu(Engine engine)
    {
      Desktop.Widgets.Clear();
      VerticalStackPanel grid = new VerticalStackPanel();
      grid.HorizontalAlignment = HorizontalAlignment.Center;
      grid.VerticalAlignment = VerticalAlignment.Center;

      grid.Spacing = 8;

      TextButton hostBtn = new TextButton();
      hostBtn.Text = "Host Game";
      hostBtn.HorizontalAlignment = HorizontalAlignment.Center;
      hostBtn.Click += (s, a) =>
      {
        HostMenu(engine);
      };
      grid.Widgets.Add(hostBtn);

      TextButton playBtn = new TextButton();
      playBtn.Text = "Join Game";
      playBtn.HorizontalAlignment = HorizontalAlignment.Center;
      playBtn.Click += (s, a) =>
      {
        JoinMenu(engine);
      };
      grid.Widgets.Add(playBtn);

      TextButton optionsBtn = new TextButton();
      optionsBtn.Text = "Options";
      optionsBtn.HorizontalAlignment = HorizontalAlignment.Center;
      optionsBtn.Click += (s, a) =>
      {
        OptionsMenu(engine);
      };
      grid.Widgets.Add(optionsBtn);

      TextButton quitBtn = new TextButton();
      quitBtn.Text = "Quit";
      quitBtn.HorizontalAlignment = HorizontalAlignment.Center;
      quitBtn.Click += (s, a) =>
      {
        engine.exit();
      };
      grid.Widgets.Add(quitBtn);


      Desktop.Widgets.Add(grid);
    }

    public static void HostMenu(Engine engine)
    {
      Desktop.Widgets.Clear();
      Grid grid = new Grid();
      grid.RowSpacing = 8;

      grid.ColumnsProportions.Add(new Proportion(ProportionType.Part));
      grid.ColumnsProportions.Add(new Proportion(ProportionType.Part));
      grid.ColumnsProportions.Add(new Proportion(ProportionType.Part));
      grid.ColumnsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Pixels, 25));
      grid.RowsProportions.Add(new Proportion(ProportionType.Pixels, 60));
      grid.RowsProportions.Add(new Proportion(ProportionType.Pixels, 25));
      grid.RowsProportions.Add(new Proportion(ProportionType.Pixels, 60));
      grid.RowsProportions.Add(new Proportion(ProportionType.Pixels, 60));

      Label nameLabel = new Label();
      nameLabel.Text = "Server name";
      nameLabel.GridColumn = 1;
      nameLabel.GridRow = 1;
      nameLabel.HorizontalAlignment = HorizontalAlignment.Left;
      grid.Widgets.Add(nameLabel);

      TextBox nameInput = new TextBox();
      nameInput.Text = "FP Server";
      nameInput.GridColumn = 2;
      nameInput.GridRow = 1;
      nameInput.HorizontalAlignment = HorizontalAlignment.Right;
      grid.Widgets.Add(nameInput);

      Label mapLabel = new Label();
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
      mapCombo.SelectedIndex = 0;
      grid.Widgets.Add(mapCombo);

      Label slotsLabel = new Label();
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

      TextButton HostBtn = new TextButton();
      HostBtn.Text = "Host";
      HostBtn.GridColumn = 1;
      HostBtn.GridRow = 4;
      HostBtn.GridColumnSpan = 2;
      HostBtn.HorizontalAlignment = HorizontalAlignment.Center;
      HostBtn.Click += (s, a) =>
      {
        if (mapCombo.SelectedItem == null)
        {
          var messageBox = Dialog.CreateMessageBox("Error", "You must select a map!");
          messageBox.ShowModal();
          return;
        }
        NetHandler.startServer(mapCombo.SelectedItem.Text);
      };
      grid.Widgets.Add(HostBtn);

      TextButton backBtn = new TextButton();
      backBtn.Text = "Back";
      backBtn.GridColumn = 1;
      backBtn.GridRow = 5;
      backBtn.GridColumnSpan = 2;
      backBtn.HorizontalAlignment = HorizontalAlignment.Center;
      backBtn.Click += (s, a) =>
      {
        MainMenu(engine);
      };
      grid.Widgets.Add(backBtn);

      Desktop.Widgets.Add(grid);
    }

    public static void JoinMenu(Engine engine)
    {
      Desktop.Widgets.Clear();
      Grid grid = new Grid();

      grid.RowSpacing = 8;

      grid.ColumnsProportions.Add(new Proportion(ProportionType.Part));
      grid.ColumnsProportions.Add(new Proportion(ProportionType.Part));
      grid.ColumnsProportions.Add(new Proportion(ProportionType.Part));
      grid.ColumnsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Pixels, 25));
      grid.RowsProportions.Add(new Proportion(ProportionType.Pixels, 60));
      grid.RowsProportions.Add(new Proportion(ProportionType.Pixels, 25));
      grid.RowsProportions.Add(new Proportion(ProportionType.Pixels, 60));
      grid.RowsProportions.Add(new Proportion(ProportionType.Pixels, 60));

      Label serverLabel = new Label();
      serverLabel.Text = "Server IP";
      serverLabel.GridColumn = 1;
      serverLabel.GridRow = 1;
      serverLabel.HorizontalAlignment = HorizontalAlignment.Left;
      grid.Widgets.Add(serverLabel);

      TextBox serverInput = new TextBox();
      serverInput.Text = "localhost";
      serverInput.GridColumn = 2;
      serverInput.GridRow = 1;
      serverInput.HorizontalAlignment = HorizontalAlignment.Right;
      grid.Widgets.Add(serverInput);

      TextButton joinBtn = new TextButton();
      joinBtn.Text = "Join";
      joinBtn.GridColumn = 1;
      joinBtn.GridRow = 2;
      joinBtn.GridColumnSpan = 2;
      joinBtn.HorizontalAlignment = HorizontalAlignment.Center;
      joinBtn.Click += (s, a) =>
      {
        Engine.netHandle = new NetHandler();
        string server = serverInput.Text;
        int port;

        if (server.Length <= 0)
        {
          Dialog messageBox = Dialog.CreateMessageBox("Error", "Bad server provided!");
          messageBox.ShowModal();
          return;
        }
        int pos = server.IndexOf(":");
        if (pos != -1) Int32.TryParse(server.Substring(pos, server.Length - pos), out port);
        else port = 27420;
        NetHandler.HandshakeCallback = () =>
              {
                engine.InitializeGame();
                engine.LoadGame();
                Engine.netHandle.GameReady = true;
                engine.toggleMouseVisible();
                engine.gstate = GameState.Playing;
                GameMenu(engine);
              };
        NetHandler.FailureCallback = () =>
              {
                engine.UnloadGame(); //Unload any loaded content
                Dialog errorBox = Dialog.CreateMessageBox("Error", "Could not reach server");
                errorBox.ShowModal();
              };
        Engine.netHandle.connect(server, port);
      };
      grid.Widgets.Add(joinBtn);

      TextButton backBtn = new TextButton();
      backBtn.Text = "Back";
      backBtn.GridColumn = 1;
      backBtn.GridRow = 3;
      backBtn.GridColumnSpan = 2;
      backBtn.HorizontalAlignment = HorizontalAlignment.Center;
      backBtn.Click += (s, a) =>
      {
        MainMenu(engine);
      };
      grid.Widgets.Add(backBtn);

      Desktop.Widgets.Add(grid);
    }

    public static void OptionsMenu(Engine engine)
    {
      Desktop.Widgets.Clear();
      Grid grid = new Grid();

      grid.RowSpacing = 8;

      grid.ColumnsProportions.Add(new Proportion(ProportionType.Part));
      grid.ColumnsProportions.Add(new Proportion(ProportionType.Part));
      grid.ColumnsProportions.Add(new Proportion(ProportionType.Part));
      grid.ColumnsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Pixels, 25));
      grid.RowsProportions.Add(new Proportion(ProportionType.Pixels, 60));
      grid.RowsProportions.Add(new Proportion(ProportionType.Pixels, 25));
      grid.RowsProportions.Add(new Proportion(ProportionType.Pixels, 60));
      grid.RowsProportions.Add(new Proportion(ProportionType.Pixels, 60));

      Label playerLabel = new Label();
      playerLabel.Text = "Player name";
      playerLabel.GridColumn = 1;
      playerLabel.GridRow = 1;
      playerLabel.HorizontalAlignment = HorizontalAlignment.Left;
      grid.Widgets.Add(playerLabel);

      TextBox playerInput = new TextBox();
      playerInput.Text = "player";
      playerInput.GridColumn = 2;
      playerInput.GridRow = 1;
      playerInput.HorizontalAlignment = HorizontalAlignment.Right;
      grid.Widgets.Add(playerInput);

      Label resolutionLabel = new Label();
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

      Label displayLabel = new Label();
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

      Label musicLabel = new Label();
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

      Label soundLabel = new Label();
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

      TextButton applyBtn = new TextButton();
      applyBtn.Text = "Apply";
      applyBtn.GridColumn = 1;
      applyBtn.GridRow = 6;
      applyBtn.GridColumnSpan = 2;
      applyBtn.HorizontalAlignment = HorizontalAlignment.Center;
      applyBtn.Click += (s, a) =>
      {
        if (displayCombo.SelectedItem == null || resolutionCombo.SelectedItem == null)
        {
          var messageBox = Dialog.CreateMessageBox("Error", "You must select a value for display/resolution!");
          messageBox.ShowModal();
          return;
        }
        Options.Config.Fullscreen = Convert.ToBoolean(displayCombo.SelectedIndex);
        Options.Config.Width = Options.Resolutions[(int)resolutionCombo.SelectedIndex].Width;
        Options.Config.Height = Options.Resolutions[(int)resolutionCombo.SelectedIndex].Height;
        Options.Config.MusicVolume = musicSlider.Value;
        Options.Config.SoundVolume = soundSlider.Value;
        Options.applyConfig();
        engine.InitializeGraphics();
        Options.Config.Save();
        OptionsMenu(engine);
      };
      grid.Widgets.Add(applyBtn);

      TextButton backBtn = new TextButton();
      backBtn.Text = "Back";
      backBtn.GridColumn = 1;
      backBtn.GridRow = 7;
      backBtn.GridColumnSpan = 2;
      backBtn.HorizontalAlignment = HorizontalAlignment.Center;
      backBtn.Click += (s, a) =>
      {
        MainMenu(engine);
      };
      grid.Widgets.Add(backBtn);

      Desktop.Widgets.Add(grid);
    }

    public static void GameMenu(Engine engine)
    {
      Desktop.Widgets.Clear();
      Grid grid = new Grid();

      grid.RowSpacing = 8;

      grid.ColumnsProportions.Add(new Proportion(ProportionType.Part));
      grid.ColumnsProportions.Add(new Proportion(ProportionType.Part));
      grid.ColumnsProportions.Add(new Proportion(ProportionType.Part));
      grid.ColumnsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Pixels, 25));
      grid.RowsProportions.Add(new Proportion(ProportionType.Pixels, 60));
      grid.RowsProportions.Add(new Proportion(ProportionType.Pixels, 25));
      grid.RowsProportions.Add(new Proportion(ProportionType.Pixels, 60));
      grid.RowsProportions.Add(new Proportion(ProportionType.Pixels, 60));

      TextButton resumeBtn = new TextButton();
      resumeBtn.Text = "Resume";
      resumeBtn.GridColumn = 1;
      resumeBtn.GridRow = 1;
      resumeBtn.HorizontalAlignment = HorizontalAlignment.Center;
      resumeBtn.Click += (s, a) =>
      {
        engine.gstate = GameState.Playing;
      };
      grid.Widgets.Add(resumeBtn);

      TextButton disconnectBtn = new TextButton();
      disconnectBtn.Text = "Disconnect";
      disconnectBtn.GridColumn = 1;
      disconnectBtn.GridRow = 2;
      disconnectBtn.HorizontalAlignment = HorizontalAlignment.Center;
      disconnectBtn.Click += (s, a) =>
      {
        engine.UnloadGame();
        if (NetHandler.Connected)
        {
          NetHandler.disconnect();
          Engine.netHandle = null;
        }
        MainMenu(engine);
      };
      grid.Widgets.Add(disconnectBtn);

      TextButton optionsBtn = new TextButton();
      optionsBtn.Text = "Options";
      optionsBtn.GridColumn = 1;
      optionsBtn.GridRow = 3;
      optionsBtn.HorizontalAlignment = HorizontalAlignment.Center;
      optionsBtn.Click += (s, a) =>
      {
        MainMenu(engine);
      };
      grid.Widgets.Add(optionsBtn);

      Desktop.Widgets.Add(grid);
    }
  }
}
