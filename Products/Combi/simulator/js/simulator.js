//Implement the automatic humidity algorithm
function calculateHumidity(temp) {
	if (temp >= 80 && temp < 100) {
		return 90;
	} else if (temp >= 100 && temp < 110) {
		return 80;
	} else if (temp >= 110 && temp < 120) {
		return 70;
	} else if (temp >= 120 && temp < 130) {
		return 60;
	} else if (temp >= 130 && temp < 150) {
		return 50;
	} else if (temp >= 150 && temp < 170) {
		return 60;
	} else if (temp >= 170 && temp < 180) {
		return 70;
	} else if (temp >= 180 && temp < 190) {
		return 80;
	} else if (temp >= 190 && temp < 260) {
		return 100;
	} else if (temp >= 260 && temp < 270) {
		return 90;
	} else if (temp >= 270 && temp < 280) {
		return 80;
	} else if (temp >= 280 && temp < 290) {
		return 70;
	} else if (temp >= 290 && temp < 300) {
		return 60;
	} else if (temp >= 300 && temp < 320) {
		return 50;
	} else if (temp >= 320 && temp < 350) {
		return 40;
	} else if (temp >= 350 && temp < 380) {
		return 30;
	} else if (temp >= 380 && temp < 410) {
		return 20;
	} else if (temp >= 410 && temp < 450) {
		return 10;
	} else if (temp == 450) {
		return 0;
	}
}

//Round to the nearest five
function round5(x) {
    return (x % 5) >= 2.5 ? parseInt(x / 5) * 5 + 5 : parseInt(x / 5) * 5;
}

function round10(x) {
	return (x % 10) >= 5 ? parseInt(x / 10) * 10 + 10 : parseInt(x / 10) * 10;
}

//Convert seconds into MM:SS format
function formatTime(seconds) {
	var mins = Math.floor(seconds / 60);
	var secs = seconds % 60;
	if (secs < 10) {
		secs = "0" + secs;
	}
	return ("0" + mins + ":" + secs);
}

//Power the countdown timer
var tick = null;
var blinkCounter = 0;

function countdown() {
	var currentTime = timeSlider.val();
	if (currentTime > 0) {
		currentTime--;
		
		timeSlider.val(currentTime);
		timeOutput.html(formatTime(currentTime));
		
		tick = setTimeout("countdown();",(currentTime > 1 ? 1500 : 500));
	} else {
		blinkCounter = 0;
		blinkTimeOutput();
		
		//beepSound.play();
		
		$("#another_batch_popup").fadeIn(750);
	}
}
function blinkTimeOutput() {
	timeOutput.fadeOut("fast").fadeIn("fast");
	blinkCounter++;
	
	if (blinkCounter < 10) {
		setTimeout("blinkTimeOutput();",100);
	}
}

//Power switch on/off
var unitOn = false;
var firstRun = true;
function powerOn() {
	unitOn = true;
	tempOutput.html(tempSlider.val() < 80 ? "---" : tempSlider.val());
	timeOutput.html(timeSlider.val() == 0 ? "--:--" : formatTime(timeSlider.val()));
	humidityOutput.html(humiditySlider.val() == 0 ? "---" : humiditySlider.val());
	
	$("#power_switch_popup").fadeOut(750);
	if (firstRun) {
		$("#temp_popup").fadeIn(750);
	}
	
	//setTimeout("showForm();",15000);
}
function powerOff() {
	unitOn = false;
	firstRun = false;
	clearTimeout(tick);
	tempOutput.html("Cln");
	timeOutput.html("Good");
	humidityOutput.html("Bye");
	setTimeout("displayOff();",1500);
}
function displayOff() {
	tempOutput.html("");
	timeOutput.html("");
	humidityOutput.html("");
}
function showForm() {
	$("#form").fadeIn(500);
}
function hideForm() {
	$("#form").fadeOut(500);
}
function hideHumidityPopup() {
	$("#humidity_popup").fadeOut(750);
	firstRun = false;
}

//Intro animation
function introAnimation() {
	$("#intro_2 .open").fadeOut(0);
	$("#intro_1 .content").delay(500).animate({
		scale:1.52,
		top:"95px",
		left:"57px"
	},1250,"linear",function() {
		$("#intro_1").fadeOut(250);
		$("#intro_2 .open").delay(750).fadeIn(250).delay(500).fadeOut(250,function() {
			$("#intro_2 .closed").animate({
				scale:2.71,
				top:"470px",
				left:"-521px"
			},2500,function() {
				$("#intro_2").fadeOut(250);
				setTimeout("directionsAnimation();",500);
			});
		});
	});
}

//Directions animation
function directionsAnimation() {
	$("#directions .power").fadeIn(375,function() {
		$("#directions .temp").delay(250).fadeIn(375,function() {
			$("#directions .time").delay(250).fadeIn(375,function() {
				$("#directions .humidity").delay(250).fadeIn(375,function() {
					$("#directions .cta").delay(250).fadeIn(375);
				});
			});
		});
	});
}

//Initialize everything
var tempSlider = null;
var tempOutput = null;
var timeSlider = null;
var timeOutput = null;
var humiditySlider = null;
var humidityOutput = null;
//var beepSound = null;
var lastTime = 0;

$(document).ready(function(e) {
	//Initialize beep sound
	//beepSound = new Audio("beep.wav");
	
	//Initialize power switch
	$("#power_switch").bind("click",function(e) {
		if (unitOn) {
			$(this).removeClass("on");
			powerOff();
		} else {
			$(this).addClass("on");
			powerOn();
		}
	});
						   
	//Initialize temperature knob
	tempSlider = $("#temp_container .slider");
	var tempMin = tempSlider.attr("min");
	var tempMax = tempSlider.attr("max");
	tempOutput = $("#temp_container .output");

    //Hide the slider
    tempSlider.hide();

	//Initialize knob
    $("#temp_container .control").knobKnob({
		snap:5,
		value:0,
		turn:function(ratio) {
			if (unitOn) {
				//Change the value of the hidden slider
				var value = Math.round(ratio * (tempMax - tempMin) + tempMin);
				if (value > 130 && value < 135) {
					var val = 132;
				} else {
					var val = round5(value);
				}
				tempSlider.val(val);
				
				//Update the output text
				if (val == 0) {
					tempOutput.html("---");
					
					//Adjust the humidity
					humiditySlider.val(0);
					humidityOutput.html("---");
				} else if (val < 370) {
					tempOutput.html(val + 80);
					
					//Adjust the humidity
					var humidity = calculateHumidity(val + 80);
					humiditySlider.val(humidity);
					humidityOutput.html(humidity);
				}
			}
		},
		begin:function() {
			$("#temp_popup").fadeOut(750);
		},
		end:function() {
			if (unitOn) {
				//Compensate for the slider not wanting to go to max
				if (tempSlider.val() == 370) {
					tempOutput.html(450);
				} else if (tempSlider.val() == 0) {
					tempOutput.html("---");
				}
				
				//Show time popup
				if (firstRun) {
					$("#time_popup").fadeIn(750);
				}
			}
		}
	});
	
	//Initialize time knob
	timeSlider = $("#time_container .slider");
	var timeMin = timeSlider.attr("min");
	var timeMax = timeSlider.attr("max");
	timeOutput = $("#time_container .output");

    //Hide the slider
    timeSlider.hide();

	//Initialize knob
    $("#time_container .control").knobKnob({
		snap:1,
		value:0,
		turn:function(ratio) {
			if (unitOn) {
				//Change the value of the hidden slider
				timeSlider.val(Math.round(ratio * (timeMax - timeMin) + timeMin));
				
				//Update the output text
				timeOutput.html(formatTime(timeSlider.val()));
			}
		},
		begin:function() {
			clearTimeout(tick);
			//beepSound.currentTime = 0;
			
			$("#time_popup").fadeOut(750);
		},
		end:function() {
			if (unitOn) {
				//Compensate for the slider not wanting to go to max
				if (timeSlider.val() == 597) {
					timeSlider.val(599);
					timeOutput.html(formatTime(599));
				}
				
				//Start counting down
				lastTime = timeSlider.val();
				tick = setTimeout("countdown();",1500);
				
				//Show humidity popup
				if (firstRun) {
					$("#humidity_popup").fadeIn(750,function() {
						setTimeout("hideHumidityPopup();",2000);
					});
				}
			}
		}
	});
	
	//Initialize humidity knob
	humiditySlider = $("#humidity_container .slider");
	var humidityMin = humiditySlider.attr("min");
	var humidityMax = humiditySlider.attr("max");
	humidityOutput = $("#humidity_container .output");

    //Hide the slider
    humiditySlider.hide();

	//Initialize knob
    $("#humidity_container .control").knobKnob({
		snap:10,
		value:0,
		turn:function(ratio) {
			if (unitOn) {
				//Change the value of the hidden slider
				var val = round10(Math.round(ratio * (humidityMax - humidityMin) + humidityMin));
				humiditySlider.val(val);
				
				//Update the output text
				if (val < 100) {
					humidityOutput.html(val);
				}
			}
		},
		end:function() {
			if (unitOn) {
				//Compensate for the slider not wanting to go to max
				if (humiditySlider.val() == 100) {
					humidityOutput.html(100);
				} else if (humiditySlider.val() == 0) {
					humidityOutput.html("---");
				}
			}
		}
	});
	
	//Initialize door & door button
	$("#door_open").fadeOut(0);
	$("#door_button").bind("click",function(e) {
		if ($(this).hasClass("close")) {
			$("#door_open").fadeOut(750,function() {
				$("#door_button").removeClass("close");
				timeSlider.val(lastTime);
				timeOutput.html(formatTime(lastTime));
			});
		} else {
			$("#door_open").fadeIn(750);
			$(this).addClass("close");
			$("#another_batch_popup").fadeOut(750);
		}
	});
	$("#another_batch_popup").fadeOut(0);
	
	//Initialize directions
	$("#power_switch_popup, #temp_popup, #time_popup, #humidity_popup, #directions > div").fadeOut(0);
	$("#directions .cta img").bind("click",function(e) {
		$("#directions").fadeOut(750,function() {
			$("#power_switch_popup").fadeIn(750);
		})
	});
	
	//Initialize form
	$("#callout").bind("click",function(evt) {
		$("#form").fadeIn(500);
	});
	$("#form").bind("click",function(evt) {
		//Check to see where this click occurred
		if (evt.target.id == "form" && (evt.offsetX <= 112 || evt.offsetX >= 523 || evt.offsetY <= 50 || evt.offsetY >= 796)) {
			$("#form").fadeOut(500);
		}
	}).fadeOut(0);
	
	//Initialize intro animation
	introAnimation();
});