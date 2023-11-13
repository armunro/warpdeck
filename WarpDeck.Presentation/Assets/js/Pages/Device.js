class Device {
    constructor(deviceId) {
        this.deviceId = deviceId;
        this.newLayerModeal = new NewLayerModal(deviceId);
        this.newLayerButton = document.getElementById("newLayerButton");
        this.saveDeviceButton = document.getElementById("saveDeviceButton");
        this.deviceNameTextbox = document.getElementById("deviceNameTextBox");
        this.newLayerButton.addEventListener('click', this.onNewLayerClick.bind(this))
        this.saveDeviceButton.addEventListener('click', this.onSaveDeviceButtonClick.bind(this))

    }

    onNewLayerClick(event) {
        this.newLayerModeal.show();
    }

    onSaveDeviceButtonClick() {
        
        let updateUri = "/api/device/" + this.deviceId;

        fetch(updateUri, {
            method: 'PUT', headers: {
                'Content-Type': 'text/json'
            },
            body: "{ \"deviceId\": \""+ this.deviceNameTextbox.value + "\"}"
        }).then(response => {
            window.location = "/device/" + this.deviceNameTextbox.value; 
        });
    }

}
