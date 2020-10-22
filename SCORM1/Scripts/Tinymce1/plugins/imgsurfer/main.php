<?php

    include 'lib/config.php';
	include 'lib/funcions.php';
	
	if(isset($_POST['submit'])){		
		if($_FILES['error'] == 0){
			move_uploaded_file($_FILES['file']['tmp_name'], BASE_RUTA.$_FILES['file']['name']);
		}
	}
	
?>
<html>
	<title>ImageSurfer</title>
	<head>
		
		<style type="text/css">
		body, td{
			font-family: verdana;
			font-size: 9px;
		}

		input{
			font-family: verdana;
			font-size: 9px;
			border: solid 1px gray;
			background-color: silver;
			padding: 2px;
		}
		
		#visor{border:solid 1px black;}
		</style>
		<script type="text/javascript">
		// funcion que pasa la ruta de la img seleccionada al textarea
		function insertUrl(url){
			
		var parentWin = (!window.frameElement && window.dialogArguments) || opener || parent || top;
		   var parentEditor = parentWin.imgsurfer_activeEditor;
		   parentEditor.execCommand('mceInsertRawHTML', false, '<img src="'+url+'">');
		   
		   top.tinymce.activeEditor.windowManager.close();
		}
		</script>
	</head>
<body>
<table width="95%" cellpadding="0" cellspacing="0" border="0" align="center">
	<tr>
		<td>&nbsp;</td>
	</tr>
	<tr>
		<td><iframe name="visor" id="visor" width="100%" height="300" src="imglist.php"></iframe>
</td>
	</tr>
	<tr>
		<td>&nbsp;</td>
	</tr>
	<tr>
		<td>
			<form name="f" method="post" enctype="multipart/form-data" action="">
			<table width="100%" cellpadding="0" cellspacing="0" border="0">
				<tr>					
					<td>Nueva imagen:</td>
					<td><input name="file" type="file" size="30"></td>										
	            <td align="right">
<input type="submit" name="submit" value="Transferir">
            	</td>
				</tr>
			</table>
			</form>
		</td>
	</tr>
	<tr>
		<td>&nbsp;</td>
	</tr>
</table>	
</body> 
</html> 
