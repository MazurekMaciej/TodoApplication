$(document).ready(function () {

  var today = new Date();
  var time = today.getHours() + ":" + today.getMinutes();
  var hours = today.getHours();
  var msg;
  if (hours < 12) msg = "Good Morning";
  else if (hours < 18) msg = "Good Afternoon";
  else msg = "Good Evening";
  document.getElementById('time').innerHTML = time;
  document.getElementById('message').innerHTML = msg;
});