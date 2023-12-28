class WarpdeckApi {

    constructor(apiBase) {
        this.apiBase = apiBase;
    }

    buildRequest(method, body, anonymous) {
        let init = {
            method: method,
            headers: {"Content-Type": "text/json"}
        }
        if (body)
            init.body = body;
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
        return fetch(`${this.apiBase}/action/test/trigger`,
            this.buildRequest("POST", JSON.stringify({
                'type': actionTypeName,
                'parameters': JSON.parse(parametersString)
            })));
    }

    getTypeProperties(parentTypeName, typeName) {
        return fetch("/api/property/" + parentTypeName + "/" + typeName)
            .then(response => {
                return response.json()
            });
    }

    getDeviceLayerComponent(deviceId, layerId, componentId) {
        return fetch(`${this.apiBase}/device/${deviceId}/layer/${layerId}/key/${componentId}`).then(value => {
            return value.json()
        })
    }
}