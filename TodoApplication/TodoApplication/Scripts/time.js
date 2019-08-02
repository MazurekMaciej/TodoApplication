$(document).ready(function () {

  var today = new Date();
  var time = today.getHours() + ":" + (mins = ('0' + today.getMinutes()).slice(-2));
  var hours = today.getHours();
  var msg;
  if (hours < 12) msg = "Good Morning";
  else if (hours < 18) msg = "Good Afternoon";
  else msg = "Good Evening";
  document.getElementById('time').innerHTML = time;
  document.getElementById('message').innerHTML = msg;
});