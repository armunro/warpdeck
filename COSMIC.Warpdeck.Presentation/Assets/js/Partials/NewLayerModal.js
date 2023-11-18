class NewLayerModal {

    constructor(deviceId) {

        this.deviceId = deviceId;
        this.registerModalEvents();
    }

    registerModalEvents() {
        document.getElementById("newLayerCreateSubmit").addEventListener('click', this.createNewLayer.bind(this))
    }

    show() {
        document.getElementById("newLayerModal").focus();
    }

    createNewLayer() {
        let newLayerTextBox = document.getElementById("newLayerName");
        let uri = "/api/device/" + this.deviceId + "/layer"
        fetch(uri, {
            method: "POST",
            headers: {
                'Content-Type': 'text/json'
            },
            body: "{\n" +
                "    \"LayerId\": \"" + newLayerTextBox.value + "\"\n" +
                "}"
        }).then(response => window.location = "/device/" + this.deviceId + "/layer/" + newLayerTextBox.value)
    }
}