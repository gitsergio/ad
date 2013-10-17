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
		
		mySqlConnection = new MySqlConnection (
			"Server=localhost;" +
			"Database=dboctubre;" +
			"User Id=root;" +
			"Password=sistemas");
                
		mySqlConnection.Open ();//ESTABLECEMOS CONEXION CON EL SERVIDOR
		
		MySqlCommand mySqlCommand = mySqlConnection.CreateCommand ();
        	mySqlCommand.CommandText = "SELECT * FROM categoria"; //CREAMOS LA SENTENCIA SQL
		
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
		
		deleteAction.Sensitive = false;
		
		deleteAction.Activated += delegate {	//ACTIVAMOS BORRADO
        	if (treeView.Selection.CountSelectedRows() == 0)
        		return;
        	TreeIter treeIter;
        	treeView.Selection.GetSelected(out treeIter);
        	object id = listStore.GetValue (treeIter, 0);
			
			MessageDialog messageDialog = new MessageDialog(this, //DIALOGO DE CONFIRMACIÓN
                DialogFlags.DestroyWithParent,
                MessageType.Question,
                ButtonsType.YesNo,
                "¿Quieres eliminar el elemento seleccionado?");
				messageDialog.Title = "Eliminar elemento";
           	 	ResponseType response = (ResponseType)messageDialog.Run ();
            	messageDialog.Destroy ();
			
            if (response == ResponseType.Yes ) {	//SI LA RESPUESTA ES SI.. BORRAR!!
            	MySqlCommand deleteMySqlCommand = mySqlConnection.CreateCommand();
            	deleteMySqlCommand.CommandText = "delete from articulo where id=" + id;
            	deleteMySqlCommand.ExecuteNonQuery();
			}
		};
	
		treeView.Selection.Changed += delegate {
        	bool hasSelectedRows = treeView.Selection.CountSelectedRows() > 0;
			deleteAction.Sensitive = hasSelectedRows;
		};
		
	}	
	
	private string[] getColumnNames(MySqlDataReader mySqlDataReader) {  //COGE LAS COLUMNAS
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