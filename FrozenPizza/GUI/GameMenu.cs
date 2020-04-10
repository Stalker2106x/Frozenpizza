using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace FrozenPizza.GUI
{
  public class GameMenu
  {
    public static Panel PlayersPlanel()
    {
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
      grid.RowsProportions.Add(new Proportion(ProportionType.Part));
      for (int bindCount = 0; bindCount < GameMain.players.Count; bindCount++)
        grid.RowsProportions.Add(new Proportion(ProportionType.Part));
      grid.RowsProportions.Add(new Proportion(ProportionType.Part));

      //Stylesheet.Current.LabelStyle.Font = Resources.regularFont;
      //Header
      Label actionLabel = new Label();
      actionLabel.Text = "Id";
      actionLabel.GridColumn = 1;
      actionLabel.GridRow = 0;
      actionLabel.HorizontalAlignment = HorizontalAlignment.Left;
      grid.Widgets.Add(actionLabel);

      Label primaryBindLabel = new Label();
      primaryBindLabel.Text = "Name";
      primaryBindLabel.GridColumn = 2;
      primaryBindLabel.GridRow = 0;
      primaryBindLabel.HorizontalAlignment = HorizontalAlignment.Left;
      grid.Widgets.Add(primaryBindLabel);

      Label secondaryBindLabel = new Label();
      secondaryBindLabel.Text = "X";
      secondaryBindLabel.GridColumn = 3;
      secondaryBindLabel.GridRow = 0;
      secondaryBindLabel.HorizontalAlignment = HorizontalAlignment.Left;
      grid.Widgets.Add(secondaryBindLabel);

      //Binds
      int i = 1;
      foreach (Player player in GameMain.players)
      {
        Label idLabel = new Label();
        idLabel.Text = player.id.ToString();
        idLabel.GridColumn = 1;
        idLabel.GridRow = i;
        idLabel.HorizontalAlignment = HorizontalAlignment.Left;
        grid.Widgets.Add(idLabel);

        Label nameLabel = new Label();
        nameLabel.Text = player.name;
        nameLabel.GridColumn = 2;
        nameLabel.GridRow = i;
        grid.Widgets.Add(nameLabel);

        Label pingLabel = new Label();
        pingLabel.Text = "50ms";
        pingLabel.GridColumn = 3;
        pingLabel.GridRow = i;
        grid.Widgets.Add(pingLabel);

        i++;
      }
      return (mainPanel);
    }
  }
}
