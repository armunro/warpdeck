class KeyGrid {
    constructor(gridElement, deviceId, layerId, data, rows=4, columns=8) {
        let dataDefaults = {
            disableSelection: false,
            disableDragDrop: false
        };
        this.deviceId = deviceId;
        this.layerId = layerId
        this.options = {...dataDefaults, ...data}
        this.gridElement = document.getElementById(gridElement)
        this.activeGridKey = null;
        this.gridRows = rows;
        this.gridColumns = columns;
        this.timerId = null;
        this.loadGrid();

    }

    registerKeyClickCallback(callback) {
        this.keyPressCallback = callback;
    }

    registerKeyHoldCallback(callback) {
        this.keyHoldCallback = callback;
    }

    registerKeyDragDropCallback(callback) {
        this.keyDragDropCallback = callback
    }

    calculateIconUrl() {
        return "/api/device/" + this.deviceId + "/layer/" + this.layerId + "/icon";
    }

    calculateLayerKeysUri() {
        return "/api/device/" + this.deviceId + "/layer/" + this.layerId + "/key"
    }

    calculateLayerKeyUri(keyId) {
        return "/api/device/" + this.deviceId + "/layer/" + this.layerId + "/key/" + keyId
    }

    loadGrid() {

        fetch(this.calculateLayerKeysUri()).then(response => {
            return response.json();
        }).then(json => {

            for (let i = 0; i < this.gridRows; i++) {
                let rowElem = document.createElement("div");
                rowElem.classList.add("row")
                for (let j = 0; j < this.gridColumns; j++) {
                    let keyId = j + (i * this.gridColumns);
                    const keyElem = this.createElement_gridKey(keyId, json[keyId]);
                    rowElem.appendChild(keyElem);
                }
                this.gridElement.appendChild(rowElem);
            }
        });
    }

    updateKey(keyId) {
        let originalKey = this.gridElement.querySelector("#key_" + keyId);
        let activate = (originalKey === this.activeGridKey)
        fetch(this.calculateLayerKeyUri(keyId)).then(response => {
            return response.status === 200 ? response.json() : null;
        }).then(json => {
            let newKey = this.createElement_gridKey(keyId, json);
            originalKey.replaceWith(newKey);
            if (activate) {
                this.onKeyClickInternal(newKey);
            }
        });

    }

    updateKeys() {
        for (let keyId = 0; keyId < (this.gridRows * this.gridColumns); keyId++) {
            this.updateKey(keyId);
        }
    }

    onKeyDragOver(event) {
        event.preventDefault();
    }

    onKeyDragStart(event) {
        let keyDiv = event.target.parentNode
        event.dataTransfer.setData("device", keyDiv.dataset.deviceid);
        event.dataTransfer.setData("layer", keyDiv.dataset.layerid);
        event.dataTransfer.setData("key", keyDiv.dataset.keyid);
    }

    onMouseDown(event) {
        if (this.keyPressCallback)
        

        this.timerId = setTimeout(() => {
            var x = event.clientX;
            var y = event.clientY;
            var elementUnder = document.elementFromPoint(x, y);
            this.keyHoldCallback(elementUnder);
            this.timerId = null;

        }, 1000); // 1000ms = 1 second

    }

    onMouseUp(event) {
        if (this.timerId) {
            clearTimeout(this.timerId);
            if (this.keyPressCallback) {
                var x = event.clientX;
                var y = event.clientY;
                var elementUnder = document.elementFromPoint(x, y);
                this.keyPressCallback(elementUnder);
            }
        }
    }

    onKeyDrop(event) {
        event.preventDefault();
        let sourceDevice = event.dataTransfer.getData("device");
        let sourceLayer = event.dataTransfer.getData("layer");
        let sourceKey = event.dataTransfer.getData("key");
        let destinationKey = event.target.parentNode.dataset.keyid
        let isCopy = event.ctrlKey;
        if (this.keyDragDropCallback)
            this.keyDragDropCallback(isCopy, sourceDevice, sourceLayer, sourceKey, destinationKey);
    }

    onKeyClickInternal(item) {
        item.classList.toggle("active")
        if (this.activeGridKey) {
            this.activeGridKey.classList.toggle("active")
        }
        this.activeGridKey = item;
        this.activeKey = {
            device: item.dataset.deviceid,
            layer: item.dataset.layerid,
            key: item.dataset.keyid,
            isBound: (item.dataset.keybound === "true")
        }
        if (this.keyPressCallback) {
            this.keyPressCallback(this.activeKey);
        }
    }

    createElement_gridKey(keyId, keyModel) {
        const keyElem = document.createElement("div");
        keyElem.id = "key_" + keyId
        keyElem.classList.add("col", "m-0", "p-0", "keyMapKey");
        keyElem.dataset.keyid = keyId;
        keyElem.dataset.deviceid = this.deviceId;
        keyElem.dataset.layerid = this.layerId;
        keyElem.dataset.keybound = (keyModel !== undefined && keyModel !== null).toString();
        keyElem.draggable = true;
        let ms = Date.now();
        keyElem.innerHTML = "<img class=\"img-fluid me-2 mb-2 keyMapKeyImage\" style=\"border-radius: 1rem\" src='" + this.calculateIconUrl() + "/" + keyId + "?ms=" + ms + "'/>";
        if (!this.options.disableSelection) {
            keyElem.addEventListener('click', () => this.onKeyClickInternal(keyElem))
        }
        if (!this.options.disableDragDrop) {
            keyElem.addEventListener('dragstart', event => this.onKeyDragStart(event));
            keyElem.addEventListener('drop', event => this.onKeyDrop(event));
            keyElem.addEventListener('dragover', event => this.onKeyDragOver(event));
            keyElem.addEventListener('mousedown', event => this.onMouseDown(event));
            keyElem.addEventListener('mouseup', event => this.onMouseUp(event));

        }
        return keyElem;
    }
}