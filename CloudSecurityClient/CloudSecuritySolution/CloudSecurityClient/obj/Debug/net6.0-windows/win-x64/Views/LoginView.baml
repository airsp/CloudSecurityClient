   M S B A M L   `   `   `  ����  CloudSecurityClient)    #CloudSecurityClient.Views.LoginView   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  云安全平台登录��5   	   $	��300��6   $	��400��6&    � WindowStartupLocation$ CenterScreen=�5   	    � 
ResizeMode$ NoResize=�5   	   .��5      � $��15q�6    � RowDefinitions 5   
   �� 5	      $
��Auto �6   �� 5
      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $��	用户名��6!   $��0,0,0,5q�6,   �� 5   
   -   5         ��Name$ txtUsername��5      $��1��6'   $��0,0,0,10q�64   �� 5   
   $��2��6   $��密码��6!   $��0,0,0,5q�6+   G� 5   
   -   5         G�Name$ txtPassword��5      $��3��6+   $��0,0,0,10q�68   �� 5   
   $��4��6   $��双因素认证码��6!   $��0,0,0,5q�6/   �� 5   
   -   5        $ txtTotp��5      $��5��6#   $��0,0,0,15q�60    �� 	MaxLength$ 6��6B   �� 5   
   -   5        $��6��5      $��登录��6   $��Center=�5      $
��20,5q�6.   5      5      