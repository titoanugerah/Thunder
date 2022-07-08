function notify(inputTitle = "Notification", inputMessage = null, inputType = "Secondary") {
	$.notify({
		icon: "flaticon-alarm-1",
		title: inputTitle,
		message: inputMessage,
	}, {
		type: inputType,
		placement: {
			from: "bottom",
			align: "right"
		},
		time: 1000,
	});
}
