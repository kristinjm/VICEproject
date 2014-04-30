// JavaScript source code

window.addEventListener("load", start, false);

function start() {
    flowrangeInput.addEventListener("change", sliderchange, false);
    rpmrangeInput.addEventListener("change", sliderchange, false);
    input1.value = flowrangeInput.value;
    alert(input1.value);
}

function sliderchange() {
    alert("slider change");
    alert(flowrangeInput.value);
    flowout.innerHTML = flowrangeInput.value;
    
    //label1.value = flowrangeInput.value;
    //input1.value = flowrangeInput.value;
    alert("done");
    //flowout.innerHTML = flowrangeInput.value;
    //rpmout.innerHTML = rpmrangeInput.value;
    //slidertext.value = rpmrangeInput.value;
}