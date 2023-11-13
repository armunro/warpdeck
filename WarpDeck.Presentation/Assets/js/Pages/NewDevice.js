class NewDevice {
    constructor() {
        this.virtualDeviceRadio = document.getElementById("virtualDeviceRadio");
        this.hardwareDeviceRadio = document.getElementById("hardwareDeviceRadio");
        this.hardwareDeviceRadio = document.getElementById("hardwareDeviceRadio");
        this.boardIdentifierSelect = document.getElementById("boardIdentifierSelect");
        this.deviceNameTextBox = document.getElementById("deviceNameTextBox");
        this.createDeviceButton = document.getElementById("createDeviceButton");
        this.hardwareDeviceInputGroup = document.getElementById("hardwareDeviceInputGroup")

        this.createDeviceButton.addEventListener('click', this.onCreateDeviceButtonClick.bind(this));
        this.virtualDeviceRadio.addEventListener('click', this.switchDeviceType.bind(this));
        this.hardwareDeviceRadio.addEventListener('click', this.switchDeviceType.bind(this));
        this.switchDeviceType();
    }
    
    onCreateDeviceButtonClick() {

        let updateUri = "/api/device/" + this.deviceNameTextBox.value;
        let hardwareId;
        hardwareId = this.virtualDeviceRadio.checked ===true ? "virtual" : this.boardIdentifierSelect.value.toString();
        let body = {
            "HardwareId": hardwareId
        };
        fetch(updateUri, {
            method: 'POST', headers: {
                'Content-Type': 'text/json'
            },
            body:JSON.stringify(body)
        }).then(response => {
            window.location = "/device/" + this.deviceNameTextBox.value;
        });
    }

    switchDeviceType() {
        if (this.virtualDeviceRadio.checked === true) {
            this.hardwareDeviceInputGroup.classList.add("d-none")
            this.hardwareDeviceInputGroup.classList.remove("d-flex")
        } else {
            this.hardwareDeviceInputGroup.classList.add("d-flex")
            this.hardwareDeviceInputGroup.classList.remove("d-none")
        }
    }
}