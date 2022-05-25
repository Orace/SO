Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports Microsoft.Xaml.Behaviors.Core

Public Class ViewModel
    Implements INotifyPropertyChanged

    Private _selectedItem As ItemViewModel

    Public Sub New()
        Items = New ObservableCollection(Of ItemViewModel) From {
            New ItemViewModel("Peter", "Parker", 22),
            New ItemViewModel("Tony", "Stark", 45)
            }

        AddCommand = New ActionCommand(AddressOf AddAction)
        RemoveCommand = New ActionCommand(AddressOf RemoveAction)
        EditCommand = New ActionCommand(AddressOf EditAction)
    End Sub

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Property AddCommand As ICommand
    Public Property RemoveCommand As ICommand
    Public Property EditCommand As ICommand

    Public Property Items As ObservableCollection(Of ItemViewModel)

    Public Property SelectedItem As ItemViewModel
        Get
            Return _selectedItem
        End Get
        Set
            _selectedItem = Value
            NotifyPropertyChanged(NameOf(SelectedItem))
        End Set
    End Property

    Private Sub AddAction()
        Items.Add(New ItemViewModel("_", "_", 0))
    End Sub

    Private Sub RemoveAction()
        If (SelectedItem IsNot Nothing) Then
            Items.Remove(SelectedItem)
        End If
    End Sub

    Private Sub EditAction()
        'TODO
    End Sub

    Private Sub NotifyPropertyChanged(propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
End Class

Public Class ItemViewModel

    Public Sub New(firstName As String, lastName As String, age As Integer)
        Me.FirstName = firstName
        Me.LastName = lastName
        Me.Age = age
    End Sub

    Public Property FirstName As String
    Public Property LastName As String
    Public Property Age As Integer
End Class