<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="WebApplication2.WebForm1" %>

<script>

    function CreatePbdControl(DivID, CLSID, ObjectID, WIDTH, HEIGHT, URL) {
        var d = parent.window.frames("PBDisplay").document.getElementById(DivID);
        d.innerHTML =
          '<object classid=' + CLSID + ' id=' + ObjectID +
          ' width=' + WIDTH + ' height=' + HEIGHT + '> <param name="DisplayURL" value="' + URL +
          '"/>';
    }

    function DsplyPB(str) {
        var Path = self.location.href
        if (Path.substr(0, 4) == "file") {
            var re = /%20/gi;
            Path = Path.replace(re, " ");
            if (Path.charAt(7) == '/')
                Path = Path.substr(8, 2) + "\\" + Path.substr(11, Path.length - 11);
        }
        lastslash = Path.lastIndexOf("/")
        var pathx
        if (lastslash != -1)
            pathx = Path.substr(0, lastslash)
        else
            pathx = Path.substr(0, 2)
        CreatePbdControl("DIV01_ID",
                         "clsid:4F26B906-2854-11D1-9597-00A0C931BFC8",
                         "pbdCtrl_01", "100%", "100%", pathx + '/' + str)
    }

    function makeCool(a) {
        a.style.fontWeight = "bold";
        a.style.cursor = "hand";
    }

    function makeNormal(a) {
        a.style.fontWeight = "normal";
    }
</script>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head>
<title>PB Menu</title>
</head>
<body>
<table>
<tr>
<td onmouseover = "makeCool(this);"
    onmouseout  = "makeNormal(this);">
  <img src="disp.bmp" border = "0" />
   PoolPumpSystem
   <button onclick="DsplyPB('PoolPumpSystem.PDI');">Display</button>
</td>
</tr>
</table>
</body>
</html>
