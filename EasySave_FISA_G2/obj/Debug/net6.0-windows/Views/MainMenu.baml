   M S B A M L   `   `   `  ����K  GProjetG2AdminDev, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:ProjetG2AdminDevProjetG2AdminDev  5      ?$clr-namespace:ProjetG2AdminDev.ViewsProjetG2AdminDev.Views  5      I)clr-namespace:ProjetG2AdminDev.ViewModelsProjetG2AdminDev.ViewModels  5      %    ProjetG2AdminDev.Views.MainMenu   5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      0localclr-namespace:ProjetG2AdminDev     5      6views$clr-namespace:ProjetG2AdminDev.Views     5      @
viewModels)clr-namespace:ProjetG2AdminDev.ViewModels     5      $
��1200��5
   8   $	��700��6E      MainViewModel  � DataContext#  [  5      .��5      �   
Background#���  6   $
��1200��65   $	��700��6B    � ColumnDefinitions 5   
   �� 5      $
��599* �6   �� 5      .+�5   
   �� $
��Left=�6   $��54��6/   $
��Wrap=�6;   $	��Top=�6O   $	��217��6g   $��48(�6s   $��	10,10,0,0q�6�   $��Bahnschrift)�6�   �� 6�    � Language$ fr-fr�6�   $�EasySave��6�   �� 6�   �� 6�   $ fr-fr�6�   6�   *   $ProjetG2AdminDev.Views.TabBackupJobs  5   
   $��0,150,0,150q�6   $	��400��63   $��2��6@   $��0��6T   �� 5   
    �� Orientation$ 
Horizontal=�6   $��0,108,0,555q�6.   $��2��6C   $��0��6W   .+�5      �� -   5         ��Name$ 	AddButton��5      $��7,0,7,0q�6Q   $��35��6b   
������ 
������ {�6(   �� *IsAddButtonEnabled+��6m   �� *AddJobCommand+.��5      �� $��../Images/plus.png��6    �� BitmapScalingMode$ HighQuality=�64   5      �� 5      -   5        $ DeleteAllButton��5      $��7,0,7,0q�6/   
������ 
������ ��6@   �� *DeleteAllJobCommand+.��5      �� $��../Images/deleteall.png��6   $��37��69   $��37��6E   $ HighQuality=�6P   5      �� 5      -   5        $ StartAllButton��5      $��7,0,7,0q�6-   
������ 
������ ��6>   �� *StartAllCommand+.��5      �� $��../Images/startall.png��6   $��37��68   $��37��6D   $ HighQuality=�6O   5      �� 5      -   5         ��Name$ 	SearchBox��5      $	��202��6)   $��27��65   $��7,0,7,0q�6B   ��6S   �� *
SearchText+ �� UpdateSourceTrigger$ PropertyChanged=��� 5      $��../Images/search.png��6   $ HighQuality=�62   $��26��6`   $��26��6l   �� 5       $��16��6   Exportation de l'historique :6C   �� 5!      -   5         ��Name$ ExportationMode��5!      $	��100��60   $��7,0, 7,0q�6=   	 �� SelectedItem	 6O   �� *Format+��6o   �� *	Formats+5"      �� 5$   
   $��0��6   $��2,572,714,91q�6%   $ 
Horizontal=�6;   .+�5%      �� -   5        $ PreviousPage��5%      $��<��6+   $��7,0, 7,0q�6:   $��35��6L   �� 5&      -   5        $ NextPage��5&      $��>��6'   $��7,0, 7,0q�63   $��35��6F   �� 5'      $��7,0, 7,0q�6   1 2 361   5(      �� 5)   
   -   5        $ SettingsButton��5)   !   $��0��6   $
��Left=�69   $��
976,10,0,0q�6T   $	��Top=�6h   $��60��6�   $	��182��6�   $��24(�6�   
������ 
������ .��5*      �� $ 
Horizontal=�6   .+�5+      �� $��../Images/settings.png��6   $��40��68   $��34��6D   �� 5,      $��Settings��6   $��38��6,   $��10q�68   5-      5.      5/      50      