   M S B A M L   `   `   `  ���� B  >DAS_GUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:WpfApp1WpfApp1      WpfApp1.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     'localclr-namespace:WpfApp1     -   $
��Main��  � Title$	  DAS��$	��350��$	��525�� � WindowState$ 	Maximized=�.��� -   $��Train��.+�� -   $��Common_Image_List��.+�� -   $��	Main_gird��.+��� -   $
��Logo��$
��Left=�$
��52.8��$��
154,10,0,0q�$	��Top=�$	��121��$��Images/로고.png���� -    ��Name$ Prebtn��$��<��$
��Left=�$��22��$��	40,28,0,0q�$	��Top=�$��32��$
��Bold"�$y�Hidden=�{�.  �� -   $ Nextbtn��$��>��$
��Left=�$��22��$��	91,28,0,0q�$	��Top=�$��32�� J� RenderTransformOrigin$ 1.906,0.682&�$
��Bold"�$y�Hidden=��� -    ��Name$
 Year��$
��Left=�$��23��$��	29,28,0,0q� �� TextWrapping$
 Wrap=�$�� ��$	��Top=�$	��120��$y�Hidden=��� -	   $ Month��$
��Left=�$��23��$��	29,28,0,0q�$
 Wrap=�$�� ��$	��Top=�$	��120��$y�Hidden=��� -
   $	 Day��$
��Left=�$��23��$��	29,28,0,0q�$
 Wrap=�$�� ��$	��Top=�$	��120��$y�Hidden=��� -   $
 Hour��$
��Left=�$��23��$��	29,28,0,0q�$
 Wrap=�$�� ��$	��Top=�$	��120��$y�Hidden=��� -   $	 Min��$
��Left=�$��23��$��	29,28,0,0q�$
 Wrap=�$�� ��$	��Top=�$	��120��$y�Hidden=��� -   $	 Sec��$
��Left=�$��23��$��	29,28,0,0q�$
 Wrap=�$�� ��$	��Top=�$	��120��$y�Hidden=��� -    ��Name$ Yearbk��$
��Left=�$��
182,50,0,0q�$
��Wrap=�$	��년��$	��Top=�$y�Hidden=��� -   $ Monbk��$
��Left=�$��
182,50,0,0q�$
��Wrap=�$	��월��$	��Top=�$y�Hidden=��� -   $ Daybk��$
��Left=�$��
182,50,0,0q�$
��Wrap=�$	��일��$	��Top=�$y�Hidden=��� -   $ Hourbk��$
��Left=�$��
182,50,0,0q�$
��Wrap=�$	��시��$	��Top=�$y�Hidden=��� -   $ Minbk��$
��Left=�$��
182,50,0,0q�$
��Wrap=�$	��분��$	��Top=�$y�Hidden=��� -   $ Secbk��$
��Left=�$��
182,50,0,0q�$
��Wrap=�$	��초��$	��Top=�$y�Hidden=�&   System.Windows.Controls.DataGrid  -     Name$ LogDataGrid��$
��Left=�$��78��$	��Top=�$	��517��$��	0,242,0,0q�   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn	   Header$	 Index��
   
IsReadOnly
 . 1  +System.Windows.Controls.DataGridBoundColumn   Binding ��  �� Path$ Index�  $	 	Train Tag��
 .  �� $	 Tag�  $	 UpDown��
 .  �� $ UpDown�  $	 
Space Name��
 .  �� $ Space�  $	 
Postion(m)��
 .  �� $ Position�  $
	 Date��
 .    Width5  /System.Windows.Controls.DataGridLengthConverter$
 Auto  �� $
 Date�o� -    o�Name$
 menu��$
��Left=�$��23��$	��Top=�$	��517��$y�Visible=�.@�m� $��프로그램��.@�m� -   $��종료��m� $��리뷰모드��$y�Visible=�.@�m� -   $��Review��$��리뷰모드 ON��m� -   $��	ReviewOff��$��리뷰모드 OFF��{�.  m� $��이력��.@�m� -   $��Print��