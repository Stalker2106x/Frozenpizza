using FrozenPizza.Network;
using FrozenPizza.Settings;
using Microsoft.Xna.Framework.Content;
using Myra.Graphics2D.TextureAtlases;
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

    public static void MainMenu()
    {
      Desktop.Widgets.Clear();

      Panel mainPanel = new Panel();
      mainPanel.Background = new TextureRegion(Collection.MenuBackground);

      VerticalStackPanel grid = new VerticalStackPanel();
      grid.HorizontalAlignment = HorizontalAlignment.Center;
      grid.VerticalAlignment = VerticalAlignment.Center;

      grid.Spacing = 8;

      TextButton hostBtn = new TextButton();
      hostBtn.Text = "Host Game";
      hostBtn.HorizontalAlignment = HorizontalAlignment.Center;
      hostBtn.Click += (s, a) =>
      {
        HostMenu();
      };
      grid.Widgets.Add(hostBtn);

      TextButton playBtn = new TextButton();
      playBtn.Text = "Join Game";
      playBtn.HorizontalAlignment = HorizontalAlignment.Center;
      playBtn.Click += (s, a) =>
      {
        JoinMenu();
      };
      grid.Widgets.Add(playBtn);

      TextButton optionsBtn = new TextButton();
      optionsBtn.Text = "Options";
      optionsBtn.HorizontalAlignment = HorizontalAlignment.Center;
      optionsBtn.Click += (s, a) =>
      {
        OptionsMenu(MainMenu);
      };
      grid.Widgets.Add(optionsBtn);

      TextButton quitBtn = new TextButton();
      quitBtn.Text = "Quit";
      quitBtn.HorizontalAlignment = HorizontalAlignment.Center;
      quitBtn.Click += (s, a) =>
      {
        Engine.quit = true;
      };
      grid.Widgets.Add(quitBtn);

      mainPanel.Widgets.Add(grid);
      Desktop.Root = mainPanel;
    }

    public static void HostMenu()
    {
      Desktop.Widgets.Clear();

      Panel mainPanel = new Panel();
      mainPanel.Background = new TextureRegion(Collection.MenuBackground);

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
        //NetHandler.startServer(mapCombo.SelectedItem.Text);
        ConnectCallback("localhost");
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
        MainMenu();
      };
      grid.Widgets.Add(backBtn);

      mainPanel.Widgets.Add(grid);
      Desktop.Root = mainPanel;
    }

    public static void JoinMenu()
    {
      Desktop.Widgets.Clear();

      Panel mainPanel = new Panel();
      mainPanel.Background = new TextureRegion(Collection.MenuBackground);

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
        ConnectCallback(serverInput.Text);
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
        MainMenu();
      };
      grid.Widgets.Add(backBtn);

      mainPanel.Widgets.Add(grid);
      Desktop.Root = mainPanel;
    }

    /// <summary>
    /// Options loader
    /// </summary>
    public static void OptionsMenu(Action prevMenu)
    {
      //LoadUIStylesheet();
      Desktop.Widgets.Clear();

      Panel mainPanel = new Panel();
      mainPanel.Background = new TextureRegion(Collection.MenuBackground);

      Grid grid = new Grid();
      int gridRow = 0;
      grid.VerticalAlignment = VerticalAlignment.Center;

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

      //Stylesheet.Current.LabelStyle.Font = Resources.regularFont;

      Label resolutionLabel = new Label();
      resolutionLabel.Text = "Resolution";
      resolutionLabel.GridColumn = 1;
      resolutionLabel.GridRow = gridRow;
      resolutionLabel.HorizontalAlignment = HorizontalAlignment.Left;
      grid.Widgets.Add(resolutionLabel);

      ComboBox resolutionCombo = new ComboBox();
      resolutionCombo.GridColumn = 2;
      resolutionCombo.GridRow = gridRow;
      resolutionCombo.HorizontalAlignment = HorizontalAlignment.Right;
      List<string> resolutions = Options.getResolutions();
      resolutions.ForEach(e => { resolutionCombo.Items.Add(new ListItem(e)); });
      resolutionCombo.SelectedIndex = Options.getDisplayMode();
      grid.Widgets.Add(resolutionCombo);
      gridRow++; //Next row

      Label displayLabel = new Label();
      displayLabel.Text = "Display mode";
      displayLabel.GridColumn = 1;
      displayLabel.GridRow = gridRow;
      displayLabel.HorizontalAlignment = HorizontalAlignment.Left;
      grid.Widgets.Add(displayLabel);

      ComboBox displayCombo = new ComboBox();
      displayCombo.GridColumn = 2;
      displayCombo.GridRow = gridRow;
      displayCombo.HorizontalAlignment = HorizontalAlignment.Right;
      displayCombo.Items.Add(new ListItem("Windowed"));
      displayCombo.Items.Add(new ListItem("Fullscreen"));
      displayCombo.SelectedIndex = Convert.ToInt32(Options.Config.Fullscreen);
      grid.Widgets.Add(displayCombo);
      gridRow++; //Next row


      Label mSensivityLabel = new Label();
      mSensivityLabel.Text = "Mouse sensivity";
      mSensivityLabel.GridColumn = 1;
      mSensivityLabel.GridRow = gridRow;
      mSensivityLabel.HorizontalAlignment = HorizontalAlignment.Left;
      grid.Widgets.Add(mSensivityLabel);

      HorizontalSlider mSensivitySlider = new HorizontalSlider();
      mSensivitySlider.GridColumn = 2;
      mSensivitySlider.GridRow = gridRow;
      mSensivitySlider.HorizontalAlignment = HorizontalAlignment.Right;
      mSensivitySlider.Minimum = 0.3f;
      mSensivitySlider.Maximum = 6f;
      mSensivitySlider.Value = Options.Config.MouseSensivity;
      grid.Widgets.Add(mSensivitySlider);
      gridRow++; //Next row

      Label gSensivityLabel = new Label();
      gSensivityLabel.Text = "Controller sensivity";
      gSensivityLabel.GridColumn = 1;
      gSensivityLabel.GridRow = gridRow;
      gSensivityLabel.HorizontalAlignment = HorizontalAlignment.Left;
      grid.Widgets.Add(gSensivityLabel);

      HorizontalSlider gSensivitySlider = new HorizontalSlider();
      gSensivitySlider.GridColumn = 2;
      gSensivitySlider.GridRow = gridRow;
      gSensivitySlider.HorizontalAlignment = HorizontalAlignment.Right;
      gSensivitySlider.Minimum = 5f;
      gSensivitySlider.Maximum = 15f;
      gSensivitySlider.Value = Options.Config.ControllerSensivity;
      grid.Widgets.Add(gSensivitySlider);
      gridRow++; //Next row

      Label musicLabel = new Label();
      musicLabel.Text = "Music volume";
      musicLabel.GridColumn = 1;
      musicLabel.GridRow = gridRow;
      musicLabel.HorizontalAlignment = HorizontalAlignment.Left;
      grid.Widgets.Add(musicLabel);

      HorizontalSlider musicSlider = new HorizontalSlider();
      musicSlider.GridColumn = 2;
      musicSlider.GridRow = gridRow;
      musicSlider.HorizontalAlignment = HorizontalAlignment.Right;
      musicSlider.Minimum = 0f;
      musicSlider.Maximum = 1f;
      musicSlider.Value = Options.Config.MusicVolume;
      grid.Widgets.Add(musicSlider);
      gridRow++; //Next row

      Label soundLabel = new Label();
      soundLabel.Text = "Sounds volume";
      soundLabel.GridColumn = 1;
      soundLabel.GridRow = 4;
      soundLabel.HorizontalAlignment = HorizontalAlignment.Left;
      grid.Widgets.Add(soundLabel);

      HorizontalSlider soundSlider = new HorizontalSlider();
      soundSlider.GridColumn = 2;
      soundSlider.GridRow = gridRow;
      soundSlider.HorizontalAlignment = HorizontalAlignment.Right;
      soundSlider.Minimum = 0f;
      soundSlider.Maximum = 1f;
      soundSlider.Value = Options.Config.SoundVolume;
      grid.Widgets.Add(soundSlider);
      gridRow++; //Next row

      gridRow++; //Skip row

      TextButton bindigsBtn = new TextButton();
      bindigsBtn.Text = "Bindings";
      bindigsBtn.GridColumn = 1;
      bindigsBtn.GridRow = gridRow;
      bindigsBtn.GridColumnSpan = 2;
      bindigsBtn.HorizontalAlignment = HorizontalAlignment.Center;
      bindigsBtn.Click += (s, a) =>
      {
        BindingsMenu(prevMenu);
      };
      grid.Widgets.Add(bindigsBtn);
      gridRow++; //Next row

      gridRow++; //Skip row
      //Stylesheet.Current.LabelStyle.Font = Resources.titleFont;

      TextButton applyBtn = new TextButton();
      applyBtn.Text = "Apply";
      applyBtn.GridColumn = 1;
      applyBtn.GridRow = gridRow;
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
        Options.Config.MouseSensivity = mSensivitySlider.Value;
        Options.Config.ControllerSensivity = gSensivitySlider.Value;
        Options.Config.MusicVolume = musicSlider.Value;
        Options.Config.SoundVolume = soundSlider.Value;
        Options.applyConfig();
        Options.Config.Save();
        prevMenu();
      };
      grid.Widgets.Add(applyBtn);
      gridRow++; //Next row

      TextButton backBtn = new TextButton();
      backBtn.Text = "Cancel";
      backBtn.GridColumn = 1;
      backBtn.GridRow = gridRow;
      backBtn.GridColumnSpan = 2;
      backBtn.HorizontalAlignment = HorizontalAlignment.Center;
      backBtn.Click += (s, a) =>
      {
        prevMenu();
      };
      grid.Widgets.Add(backBtn);

      mainPanel.Widgets.Add(grid);
      Desktop.Root = mainPanel;
    }

    /// <summary>
    /// Bindings Menu loader
    /// </summary>
    public static void BindingsMenu(Action optionsPrevMenu)
    {
      //LoadUIStylesheet();
      Desktop.Widgets.Clear();

      Panel mainPanel = new Panel();
      mainPanel.Background = new TextureRegion(Collection.MenuBackground);

      Grid grid = new Grid();
      grid.VerticalAlignment = VerticalAlignment.Center;

      grid.RowSpacing = 8;

      grid.ColumnsProportions.Add(new Proportion(ProportionType.Part));
      grid.ColumnsProportions.Add(new Proportion(ProportionType.Part));
      grid.ColumnsProportions.Add(new Proportion(ProportionType.Part));
      grid.ColumnsProportions.Add(new Proportion(ProportionType.Part));
      grid.ColumnsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Part));
      for (int bindCount = 0; bindCount < Options.Config.Bindings.Count; bindCount++)
        grid.RowsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Pixels, 75));
      grid.RowsProportions.Add(new Proportion(ProportionType.Pixels, 60));
      grid.RowsProportions.Add(new Proportion(ProportionType.Pixels, 60));

      //Stylesheet.Current.LabelStyle.Font = Resources.regularFont;
      //Header
      Label actionLabel = new Label();
      actionLabel.Text = "Action";
      actionLabel.GridColumn = 1;
      actionLabel.GridRow = 0;
      actionLabel.HorizontalAlignment = HorizontalAlignment.Left;
      grid.Widgets.Add(actionLabel);

      Label primaryBindLabel = new Label();
      primaryBindLabel.Text = "Primary";
      primaryBindLabel.GridColumn = 2;
      primaryBindLabel.GridRow = 0;
      primaryBindLabel.HorizontalAlignment = HorizontalAlignment.Left;
      grid.Widgets.Add(primaryBindLabel);

      Label secondaryBindLabel = new Label();
      secondaryBindLabel.Text = "Secondary";
      secondaryBindLabel.GridColumn = 3;
      secondaryBindLabel.GridRow = 0;
      secondaryBindLabel.HorizontalAlignment = HorizontalAlignment.Left;
      grid.Widgets.Add(secondaryBindLabel);

      //Binds
      int i = 1;
      foreach (KeyValuePair<GameAction, ControlPair> entry in Options.Config.Bindings)
      {
        Label action = new Label();
        action.Text = entry.Key.ToString();
        action.GridColumn = 1;
        action.GridRow = i;
        action.HorizontalAlignment = HorizontalAlignment.Left;
        grid.Widgets.Add(action);

        TextButton primaryBind = new TextButton();
        primaryBind.Text = entry.Value.primary.GetValueOrDefault().GetInput();
        primaryBind.GridColumn = 2;
        primaryBind.GridRow = i;
        primaryBind.Click += (s, a) =>
        {
          (new Thread(() => {
            primaryBind.Text = "...";
            Control? control;
            int attempts = 30;
            do
            {
              control = Control.GetAnyPressedKey(Engine.getDeviceState());
              Thread.Sleep(100);
              attempts--;
            } while (control == null && attempts > 0);
            if (control != null)
            {
              var bind = Options.Config.Bindings[entry.Key];
              bind.primary = control;
              Options.Config.Bindings[entry.Key] = bind;
            }
            primaryBind.Text = Options.Config.Bindings[entry.Key].primary.GetValueOrDefault().GetInput();
          })).Start();
        };
        grid.Widgets.Add(primaryBind);

        TextButton secondaryBind = new TextButton();
        secondaryBind.Text = entry.Value.secondary.GetValueOrDefault().GetInput();
        secondaryBind.GridColumn = 3;
        secondaryBind.GridRow = i;
        secondaryBind.Click += (s, a) =>
        {
          (new Thread(() => {
            secondaryBind.Text = "...";
            Control? control;
            int attempts = 30;
            do
            {
              control = Control.GetAnyPressedKey(Engine.getDeviceState());
              Thread.Sleep(100);
              attempts--;
            } while (control == null && attempts > 0);
            if (control != null)
            {
              var bind = Options.Config.Bindings[entry.Key];
              bind.secondary = control;
              Options.Config.Bindings[entry.Key] = bind;
            }
            secondaryBind.Text = Options.Config.Bindings[entry.Key].secondary.GetValueOrDefault().GetInput();
          })).Start();
        };
        grid.Widgets.Add(secondaryBind);

        i++;
      }

      //Stylesheet.Current.LabelStyle.Font = Resources.titleFont;
      TextButton applyBtn = new TextButton();
      applyBtn.Text = "Apply";
      applyBtn.GridColumn = 1;
      applyBtn.GridRow = i + 1;
      applyBtn.GridColumnSpan = 3;
      applyBtn.HorizontalAlignment = HorizontalAlignment.Center;
      applyBtn.Click += (s, a) =>
      {
        OptionsMenu(optionsPrevMenu);
      };
      grid.Widgets.Add(applyBtn);

      TextButton backBtn = new TextButton();
      backBtn.Text = "Cancel";
      backBtn.GridColumn = 1;
      backBtn.GridRow = i + 2;
      backBtn.GridColumnSpan = 3;
      backBtn.HorizontalAlignment = HorizontalAlignment.Center;
      backBtn.Click += (s, a) =>
      {
        OptionsMenu(optionsPrevMenu);
      };
      grid.Widgets.Add(backBtn);

      mainPanel.Widgets.Add(grid);
      Desktop.Root = mainPanel;
    }

    public static void GameMenu()
    {
      Desktop.Widgets.Clear();

      Panel mainPanel = new Panel();
      mainPanel.Background = new TextureRegion(Collection.MenuBackground);

      VerticalStackPanel grid = new VerticalStackPanel();
      grid.HorizontalAlignment = HorizontalAlignment.Center;
      grid.VerticalAlignment = VerticalAlignment.Center;

      grid.Spacing = 8;

      TextButton resumeBtn = new TextButton();
      resumeBtn.Text = "Resume";
      resumeBtn.HorizontalAlignment = HorizontalAlignment.Center;
      resumeBtn.Click += (s, a) =>
      {
        Engine.setState(GameState.Playing);
      };
      grid.Widgets.Add(resumeBtn);

      TextButton disconnectBtn = new TextButton();
      disconnectBtn.Text = "Disconnect";
      disconnectBtn.HorizontalAlignment = HorizontalAlignment.Center;
      disconnectBtn.Click += (s, a) =>
      {
        GameMain.Unload();
        Engine.networkClient.disconnect();
        MainMenu();
      };
      grid.Widgets.Add(disconnectBtn);

      TextButton optionsBtn = new TextButton();
      optionsBtn.Text = "Options";
      optionsBtn.HorizontalAlignment = HorizontalAlignment.Center;
      optionsBtn.Click += (s, a) =>
      {
        OptionsMenu(GameMenu);
      };
      grid.Widgets.Add(optionsBtn);

      mainPanel.Widgets.Add(grid);
      Desktop.Root = mainPanel;
    }

    public static void ConnectCallback(string host)
    {
      int port;

      if (host.Length <= 0)
      {
        Dialog messageBox = Dialog.CreateMessageBox("Error", "Bad server provided!");
        messageBox.ShowModal();
        return;
      }
      int pos = host.IndexOf(":");
      if (pos != -1) Int32.TryParse(host.Substring(pos, host.Length - pos), out port);
      else port = 27015;
      /*Engine.netHandle = new NetHandler();
      NetHandler.HandshakeCallback = () =>
      {
        GameMain.Initialize(engine.GraphicsDevice);
        GameMain.Load(engine.Content);
        Engine.netHandle.GameReady = true;
        engine.toggleMouseVisible();
        Engine.setState(GameState.Playing);
      };
      NetHandler.FailureCallback = () =>
      {
        GameMain.Unload(); //Unload any loaded content
        Dialog errorBox = Dialog.CreateMessageBox("Error", "Could not reach server");
        errorBox.ShowModal();
      };*/
      Engine.networkClient = new ClientV2();
      Engine.networkClient.connect(host, port);
    }
  }
}
