using Gtk;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

public partial class MainWindow: Gtk.Window
{	
	private MySqlConnection mySqlConnection;
	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
		
		mySqlConnection = new MySqlConnection("Server=localhost;Database=dbprueba;User Id=root;Password=sistemas");
		mySqlConnection.Open ();
		
		MySqlCommand mySqlCommand = mySqlConnection.CreateCommand ();
		mySqlCommand.CommandText = 
			"select a.id, a.nombre, c.nombre as categoria, a.precio " +
			"from articulo a left join categoria c " +
			"on a.categoria = c.id ";
		
		MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
		
		string[] columnNames = getColumnNames(mySqlDataReader);
		
		appendColumns(columnNames);
		
		ListStore listStore = createListStore(mySqlDataReader.FieldCount);

		while (mySqlDataReader.Read ()) {
			List<string> values = new List<string>();
			for (int index = 0; index < mySqlDataReader.FieldCount; index++)
				values.Add ( mySqlDataReader.GetValue (index).ToString() );
			listStore.AppendValues(values.ToArray());
		}
		mySqlDataReader.Close ();
		
		treeView.Model = listStore;
		
		editAction.Sensitive = false;
		deleteAction.Sensitive = false;
		
		editAction.Activated += delegate {
			if (treeView.Selection.CountSelectedRows() == 0)
				return;
			TreeIter treeIter;
			treeView.Selection.GetSelected(out treeIter);
			object id = listStore.GetValue (treeIter, 0);
			object nombre = listStore.GetValue (treeIter, 1);
			
			MessageDialog messageDialog = new MessageDialog(this,
                DialogFlags.DestroyWithParent,
                MessageType.Info,
                ButtonsType.Ok,
                "Seleccionado Id={0} Nombre={1}", id, nombre);
			messageDialog.Title = "Este es el título del mensaje";
			messageDialog.Run ();
			messageDialog.Destroy ();
		};
		
		deleteAction.Activated += delegate {
			if (treeView.Selection.CountSelectedRows() == 0)
				return;
			TreeIter treeIter;
			treeView.Selection.GetSelected(out treeIter);
			object id = listStore.GetValue (treeIter, 0);
			
			MessageDialog messageDialog = new MessageDialog(this,
                DialogFlags.DestroyWithParent,
                MessageType.Question,
                ButtonsType.YesNo,
                "¿Quieres eliminar el elemento seleccionado?");
			messageDialog.Title = "Eliminar elemento";
			ResponseType response = (ResponseType)messageDialog.Run ();
			messageDialog.Destroy ();
			if (response == ResponseType.Yes ) {
				MySqlCommand deleteMySqlCommand = mySqlConnection.CreateCommand();
				deleteMySqlCommand.CommandText = "delete from articulo where id=" + id;
				deleteMySqlCommand.ExecuteNonQuery();
			}
		};
		
		treeView.Selection.Changed += delegate {
			bool hasSelectedRows = treeView.Selection.CountSelectedRows() > 0;
			editAction.Sensitive = hasSelectedRows;
			deleteAction.Sensitive = hasSelectedRows;
		};
		
		//treeView.Selection.CountSelectedRows()
//		treeView.Selection.Changed += delegate {
//			TreeIter treeIter;
//			Console.WriteLine ("============");
//			if (treeView.Selection.GetSelected (out treeIter)) {
//				Console.WriteLine ("listStore.GetPath(treeIter)=" + listStore.GetPath(treeIter) );
//				Console.WriteLine ("listStore.GetValue(treeIter, 0)=" + listStore.GetValue(treeIter, 0));
//				Console.WriteLine ("listStore.GetValue(treeIter, 1)=" + listStore.GetValue(treeIter, 1));
//			} else
//				Console.WriteLine ("Ninguno seleccionado");
//		};
	}
	
	private string[] getColumnNames(MySqlDataReader mySqlDataReader) {
		List<string> columnNames = new List<string>();
		for (int index = 0; index < mySqlDataReader.FieldCount; index++)
			columnNames.Add (mySqlDataReader.GetName (index));
		return columnNames.ToArray ();
	}
	
	private void appendColumns(string[] columnNames) {
		int index = 0;
		foreach (string columnName in columnNames) 
			treeView.AppendColumn (columnName, new CellRendererText(), "text", index++);
	}
	
	private ListStore createListStore(int fieldCount) {
		Type[] types = new Type[fieldCount];
		for (int index = 0; index < fieldCount; index++)
			types[index] = typeof(string);
		return new ListStore(types);
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
		
		mySqlConnection.Close ();
	}
}
