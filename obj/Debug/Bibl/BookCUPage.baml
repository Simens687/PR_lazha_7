   M S B A M L   `   `   `  ����B  >IS_Bibl, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:IS_Bibl.BiblIS_Bibl.Bibl  5          IS_Bibl.Bibl.BookCUPage   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ,localclr-namespace:IS_Bibl.Bibl     5      _ [MaterialDesignThemes.Wpf, Version=5.1.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5	       #  MaterialDesign.Brush.Foreground#���   5
       # MaterialDesign.Brush.Background  N� 
Background#  �  5      $��Medium"�5      $��20(�5       N� Title$ ReadPage��5       N� 
FontFamily 5      :  4MaterialDesignThemes.Wpf.MaterialDesignFontExtension  .��5      �  � ColumnDefinitions 5   
   �� 5      $
��0.1* �6   �� 5      $��1* �6   �� 5      $
��0.1* �6   �� 5      $��1* �6   �� 5      $
��0.1* �6   �� 5      $
��0.5* �6   �� 5      $
��0.5* �6   �� 5      $
��0.1* �6    � RowDefinitions 5   
   �� 5      $��* �6   �� 5       $��* �6   �� 5!      $��* �6   �� 5"      $��* �6   �� 5#      $��* �6   .+�5&   
   �� -   5         ��Name$ TbxName��5&      $��1��6#   $��0��63   $��3��6@   $��Center=�6T   )  #MaterialDesignThemes.Wpf.HintAssist   Hint$ Название��5'        MaterialDesignFilledTextBox#��[ 5(      .��66   5)      �� 5+   
   -   5        $ TbxDescription��5+      $��1��6*   $��1��6:   $��3��6G   $��Center=�6[   $ Описание��5,      #��[ 5-      .��66   5.      �� 50   
   -   5        $ TbxDate��50      $��5��6#   $��1��63   $��2��6@   $��Center=�6T   $# Дата публикации��51      #��[ 52      .��66   53      �� 55   
   -   5        $ TbxQuantity��55      $��5��6'   $��2��67   $��2��6D   $��Center=�6X   $ Количество��56      #��[ 57      .��66   58      &   System.Windows.Controls.DataGrid  5:   
   -   5          Name$ 	DGAuthors��5:      $��18(�6&   $��1��64   $��2��6D   $��2��6Q      CanUserAddRows .  	   SelectionMode$	 Extended=�5<      
   SelectionUnit$
 FullRow=�5=         
IsReadOnly . 5>        5@   
   -   5        $ DGGenres��5@      $��18(�6%   $��3��63   $��2��6C   $��2��6P    .  $	 Extended=�5B      $
 FullRow=�5C       . 5D      �� 5F   
   -   5         ��Name$ 	BtnCreate��5F      $��5��6$   $��4��64   $��Center=�6A   $��Center=�6e   $��Сохранить��5G      $��20(�6    + 'MaterialDesignFlatSecondaryDarkBgButton#��[ 5I       � ToolTip$- 'MaterialDesignFlatSecondaryDarkBgButton��5J      {�5H      �� *DataContext.ControlsEnabled+ �� RelativeSource �� *FindAncestor+ �� AncestorType$ WindowL�5J   3   �� 5L   
   -   5        $ BtnExit��5L      $��6��6"   $��4��62   $��Center=�6?   $��Center=�6c   $��Отменить��5M      $��20(�6   #��[ 5O      $- 'MaterialDesignFlatSecondaryDarkBgButton��5P      {�5N      �� *DataContext.ControlsEnabled+ �� *FindAncestor+$ WindowL�5P   3   5Q      5R      