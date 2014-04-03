// JavaScript source code
alert("starting script");
window.addEventListener("load", start, false);

function start() {
    setSched.addEventListener("click", schedbuttonClick, false);
    var content = "Select Time";
    for (var i = 0; i < 24; i++)
    {
        content += "<asp:ListItem>" + hours[i] + "</asp:ListItem>";
    }
    document.getElementById("vfdstart").innerHTML = content;
    vfdstart.innerHTML = content;
    alert("vfd start changed");

}

