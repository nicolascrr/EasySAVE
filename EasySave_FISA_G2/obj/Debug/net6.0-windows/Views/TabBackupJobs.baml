   M S B A M L   `   `   `  ����K  GProjetG2AdminDev, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:ProjetG2AdminDev.ViewsProjetG2AdminDev.Views  5      b ^PresentationFramework.Aero2, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35hKclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.Aero2Microsoft.Windows.Themes 5      *    $ProjetG2AdminDev.Views.TabBackupJobs   5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      6views$clr-namespace:ProjetG2AdminDev.Views     5      ^themesKclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.Aero2     5      $	��400��5	   8   $
��1150��6E      MainViewModel  � DataContext#  [  5
       � 	Resources 5       �� MergedDictionaries �� Source9   3ProjetG2AdminDev.Views.BooleanToVisibilityConverter  BoolToVisibilityConverter8   2ProjetG2AdminDev.Views.StringToVisibilityConverter  StringToVisibilityConverter  DataGridStyle �� 
TargetType&   System.Windows.Controls.DataGrid �� Property �� Value)  #System.Windows.Controls.DataGridRow �� Triggers P� Property	 P� Value*  $System.Windows.Controls.DataGridCell
 �� 
TargetType �� Padding�� 5   
    5      �� 5      $" /Properties/ColorsLight.xaml?�6%   %}  &	       &	      &	 &       5      61     5      60   �� 5      "  6*   .�5      ��  � �� �#DDDDDD60   �� 5   
     �@ HorizontalGridLinesBrush �#DDDDDD65   �� 5   
    �@ VerticalGridLinesBrush �#DDDDDD63   �� 5   
    �@ RowStyle 5      �� 5      "  6    5      P� 5       �@J�IsMouseOver$
	 True��69   . �5      ��  � ��
 �����6;   5      5!      5#      �� 5$   
    �@ 	CellStyle 5%      �� 5&      "  6    5'      P� 5)       �@ 
IsSelected$
	 True��68   . �5*      ��  � �� �	#ff66ccff6;   �� 5+       � ��
 ���� 6<   �� 5,       � ��
 �   �6;   5-      P� 5.       �@J�	IsFocused$
	 True��67   . �5/      ��  � �� �	#ff66ccff6;   �� 50       � ��
 �   �6<   �� 51       � ��$ 1q�6@   �� 52       � ��
 �   �6;   53      .�56      ��  � ��$ 4,3,4,3q�60   �� 57       � ��
 ���� 64   �� 58       � ��$ 1q�68   �� 59       � �� 5:      �� 5;      "
  6.   .R�5<   "   �� # z��6)   #��z��5=   -   #��z��5>   -   #��z��5?   -   ._�5@   &   �� 5A   #   5B      5D      �� 5E       �@�FocusVisualStyle 5F      �� 5G      " ��6$   .�5H   "   ��  � ��
 ���� 6@   5I      5K      5L      5N      5O      5P      .��5S      � $
��1150��6   .+�5T   
     -   5          Name$ BackupJobDataGrid��5T         AutoGenerateColumns$ False��6.   $
��1150��6J      CanUserAddRows$ False��5U   6   #��[ 5V   E   ��5U      �� *
BackupJobs+ 5W        
Background��%H  '        �� 5X      "  6   .�5Y      ��  � ��
 �����63   �� 5Z       � ��
 �   �63    5[      P� 5\       �@ 	IsEditing$
	 True��67   . �5]      ��  � ��
 � � �6;   5^      P� 5_       �@ 
IsSelected$
	 True��68   . �5`      ��  � ��# �  6;   5a      5c      5d         Columns 5e      0  *System.Windows.Controls.DataGridTextColumn  5f      ,  &System.Windows.Controls.DataGridColumn   Header$	 Nom��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$	 10* 6K   1	  +System.Windows.Controls.DataGridBoundColumn 	  Binding 62   �� *Name+6K   4
  .System.Windows.Controls.DataGridTemplateColumn
  5g      $ Source��6)   $	 25* 69    
  CellTemplate 5h      �� 5i      .R�5j      ��  �� Orientation$ Vertical=�6)   $��
0 ,3, 0, 3q�6@   .+�5k   "   �� $��Sélectionner��6)   $��7,0, 7,0q�6@   ��5l   )   �� *%#DataContext.BrowseSourcePathCommand+ �� ElementName$ BackupJobDataGrid����5m   )   �� �� 5n   "   ��6,   �� *
SourcePath+5o      5p      5r      
  5s      $ Destination��6)   $	 25* 6>    5t      �� 5u      .R�5v      �� $ Vertical=�6)   $��
0 ,3, 0, 3q�6@   .+�5w   "   �� $��Sélectionner��6)   $��7,0, 7,0q�6@   ��5x   )   �� **(DataContext.BrowseDestinationPathCommand+$ BackupJobDataGrid����5y   )   �� �� 5z   "   ��6,   �� *DestinationPath+5{      5|      5~      
  5      $
 Type��6)    5�      �� 5�      .R�5�      ��  �� SelectedItem 6'   �� *Type+��5�   '   �� *DataContext.Types+$ BackupJobDataGrid��5�      5�      
  5�      $ Actions��6)   $	 50* 6:    5�      �� 5�      .R�5�      �� $ 
Horizontal=�6)   $��
0 ,3, 0, 3q�6B   .+�5�   "   �� $��7,0, 7,0q�6)   
������ 
������ $��25��5�   \   ��5�   )   �� *DataContext.StartJobCommand+$ BackupJobDataGrid����5�   )   �� y�5�   )   �� *State+ �� 	Converter# [  �� ConverterParameter$ False��.��5�   &   �� $��../Images/start.png��6,    �� BitmapScalingMode$ HighQuality=�6I   5�   #   �� 5�   "   $��7,0, 7,0q�6)   
������ 
������ $��25��5�   \   ��5�   )   �� *DataContext.PauseJobCommand+$ BackupJobDataGrid����5�   )   �� y�5�   )   �� *State+# [ $
 True��.��5�   &   �� $��../Images/pause.png��6,   $ HighQuality=�6I   5�   #   �� 5�   "   $��7,0, 7,0q�6)   
������ 
������ $��25��5�   \   ��5�   )   �� *DataContext.StopJobCommand+$ BackupJobDataGrid����5�   )   �� y�5�   )   �� *	IsRunning+# [ .��5�   &   �� $��../Images/stop.png��6,   $ HighQuality=�6H   5�   #   �� 5�   "   $��7,0, 7,0q�6)   
������ 
������ $��25��5�   )   ��5�   )   �� *DataContext.DeleteJobCommand+$ BackupJobDataGrid����5�   )   �� .��5�   &   �� $��../Images/delete_.png��6,   $ HighQuality=�6K   5�   #   � 5�   "   $��18��6.   $	��140��6:   $��7,0, 7,0q�5�   .    � Value 5�   F   �� *
Progress+ �� Mode$ OneWay=�y�5�   .   �� *	IsRunning+# [ 5�      5�      5�      
  5�      $ Priority��6)   $ 5* 6;    5�      �� 5�      .R�5�      �� $ 
Horizontal=�6)   $��
0 ,3, 0, 3q�6B   .+�5�   "   �� ��6(   �� *HasPriority+5�      5�      5�      5�      5�      5�      