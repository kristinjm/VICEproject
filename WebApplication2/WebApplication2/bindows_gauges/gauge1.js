// JavaScript source code

// Load the gauge into the div
var gauge_rpm = bindows.loadGaugeIntoDiv("gauges/rpm_gauge.xml", "gaugeDivrpm");

var t1 = 0;
var interval1 = 1;
var rpm = "<%=RPM.value%>";

function updateGauge1() {
    if (rpm >= t1) {
        if (t1 == rpm) { return }
        t1 += interval1;
        gauge_rpm.needle.setValue(t1);
        gauge_rpm.label.setText(Math.round(t1));
    }
    if (rpm < t1) {
        if (t1 == rpm) { return }
        t1 -= interval1;
        gauge_rpm.needle.setValue(t1);
        gauge_rpm.label.setText(Math.round(t1));
    }
}
setInterval(updateGauge1, interval1);   //1000 == 1 second