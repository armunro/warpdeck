class WarpdeckApi {

    constructor(apiBase) {
        this.apiBase = apiBase;
    }

    buildRequest(method, body, anonymous) {
        let init = {
            method: method, headers: {"Content-Type": "text/json"}
        }
        if (body) init.body = body;
        if (!anonymous) {
            //Uncomment when 
            //init.withCredentials = true;
            //init.credentials = "include";
            //init.headers.Authorization = `Bearer ${this.token}`;

        }
        return init;
    }

    getClips() {
        return fetch(`${this.apiBase}/clipboard`).then(value => {
            return value.json()
        });
    }

    triggerAction(actionTypeName, parametersString) {
        return fetch(`${this.apiBase}/action/test/trigger`, this.buildRequest("POST", JSON.stringify({
            'type': actionTypeName, 'parameters': JSON.parse(parametersString)
        })));
    }

    getTypeProperties(parentTypeName, typeName) {
        return fetch(`${this.apiBase}/property/${parentTypeName}/${typeName}`)
            .then(response => {
                return response.json()
            });
    }

    getDeviceLayerComponent(deviceId, layerId, componentId) {
        return fetch(`${this.apiBase}/device/${deviceId}/layer/${layerId}/key/${componentId}`).then(value => {
            return value.json()
        });
    }

    getComponentTriggers() {
        return fetch(`${this.apiBase}/behavior/PressAndHold/actions`).then(value => {
            return value.json();
        });
    }

    createDeviceLayerComponent(deviceId, layerId, componentId) {
        let newKeyBody = `{
    "KeyId": "${componentId}",
    "Behavior": {
        "Type": "Press",
        "Actions": {
            "Press": {
                "Type": "KeyMacro",
                "Parameters": {
                    "keys": "(control+alt+shift+m)|(o)"
                }
            }
        }
    },
    "Properties": {
        "Category" : "",
        "Text" :"New",
        "Icon":  ""
    }
}`

        return fetch(`${this.apiBase}/device/${deviceId}/layer/${layerId}/key/${componentId}`, this.buildRequest('POST', JSON.stringify(newKeyBody))).then(value => {
            return value.json()
        });

    }

    saveDeviceLayerComponent(deviceId, layerId, componentId, model) {
        return fetch(`${this.apiBase}/device/${deviceId}/layer/${layerId}/key/${componentId}`, this.buildRequest('PUT', JSON.stringify(model))).then(value => {
            return value.json()
        });
    }

    deleteDeviceLayerComponent(deviceId, layerId, keyId) {
        return fetch(`${this.apiBase}/device/${deviceId}/layer/${layerId}/key/${keyId}`, this.buildRequest("DELETE"))
    }

    copyDeviceLayerComponent(deviceId, layerId, componentId, targetComponentId) {
        return fetch(`${this.apiBase}/device/${deviceId}/layer/${layerId}/key/${componentId}/copy/${targetComponentId}`);
    }

    moveDeviceLayerComponent(deviceId, layerId, componentId, targetComponentId) {
        return fetch(`${this.apiBase}/device/${deviceId}/layer/${layerId}/key/${componentId}/move/${targetComponentId}`);
    }

    createDevice(name, hardware) {
        return fetch(`${this.apiBase}"/device/" + name;`, this.buildRequest("POST", JSON.stringify({
            "HardwareId": hardware
        })));
    }

    createDeviceLayer(deviceId, newLayerId) {
        return fetch(`${this.apiBase}/device/${deviceId}/layer`, this.buildRequest("POST", JSON.stringify({"LayerId": newLayerId})));
    }

    triggerComponentPress(deviceId, componentId) {
        return fetch(`${this.apiBase}/device/${deviceId}/keys/${componentId}/press`, {method: "POST"}).then((response) => {
            return response.json();
        });
    }

    getAvailableHardware() {
        return fetch(`${this.apiBase}/hardware/available`).then(response => {
            return response.json();
        })
    }

    triggerComponentHold(deviceId, componentId) {
        return fetch(`${this.apiBase}/device/${deviceId}/keys/${componentId}/hold`, {method: "POST"}).then((response) => {
            return response.json();
        });
    }

}