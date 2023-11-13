class EditLayer {
    constructor(KeyGrid) {
        this.activeModel = null;
        this.keyGrid = KeyGrid;
        this.propertiesCard = document.getElementById("propertiesCard");
        this.createCard = document.getElementById("createKeyCard");
        this.saveKeyButton = document.getElementById("saveKey");
        this.newKeyButton = document.getElementById("newKey");
        this.deleteKeyButton = document.getElementById("deleteKey");
        this.addPropertyNameSelect = document.getElementById("addPropertyNameSelect");
        this.addPropertyValueTextBox = document.getElementById("addPropertyValueTextBox");
        this.addPropertyButton = document.getElementById("addPropertyButton");
        this.tagContainer = document.getElementById("tagContainer");
        this.behaviorSelect = document.getElementById("behaviorSelect")
        this.actionContainer = document.getElementById("actionContainer")

        //events
        this.keyGrid.registerKeyClickCallback(this.onKeyGridKeyClick.bind(this));
        this.keyGrid.registerKeyDragDropCallback(this.onGridKeyDragDrop.bind(this));
        this.saveKeyButton.addEventListener('click', this.onSaveKeyClick.bind(this));
        this.deleteKeyButton.addEventListener('click', this.onDeleteKeyClick.bind(this));
        this.newKeyButton.addEventListener('click', this.onNewKeyClick.bind(this));
        this.addPropertyButton.addEventListener('click', this.onAddPropertyClick.bind(this))
    }

    onAddPropertyClick() {
        this.activeModel.Properties[this.addPropertyNameSelect.value] = this.addPropertyValueTextBox.value;
        let keyPropertiesPromise = this.getProperties("KeyBehavior", this.activeModel.Behavior["Type"]);
        let iconPropertiesPromise = this.getProperties("IconTemplate", this.activeModel.Behavior["Type"]);
        Promise.all([keyPropertiesPromise, iconPropertiesPromise]).then((properties) => {
            this.bindPropertyEditors(this.activeModel, properties);
            this.bindAddPropertyEditors(this.activeModel, properties)
        })

    }

    onNewKeyClick() {
        let activeKey = this.keyGrid.activeKey;
        let updateUri = "/api/device/" + activeKey.device
            + "/layer/" + activeKey.layer + "/key";

        let newKeyBody = "{\n" +
            "    \"KeyId\": " + activeKey.key + ",\n" +
            "    \"Behavior\": {\n" +
            "        \"Type\": \"Press\",\n" +
            "        \"Actions\": {\n" +
            "            \"press\": {\n" +
            "                \"Type\": \"KeyMacro\",\n" +
            "                \"Parameters\": {\n" +
            "                    \"keys\": \"(control+alt+shift+m)|(o)\"\n" +
            "                }\n" +
            "            }\n" +
            "        }\n" +
            "    },\n" +
            "    \"Properties\": {\n" +
            "        \"key.category\" : \"Rider-General\",\n" +
            "        \"text\" :\"Close Other\",\n" +
            "        \"graphic.path\":  \"list-check.svg\"\n" +
            "    }\n" +
            "}"
        fetch(updateUri, {
            method: 'POST', headers: {
                'Content-Type': 'text/json'
            },
            body: newKeyBody
        }).then(response => {
            this.keyGrid.updateKey(activeKey.key);
        });


    }

    onGridKeyDragDrop(isCopy, sourceDevice, sourceLayer, sourceKey, destinationKey) {
        let operation = isCopy ? "copy" : "move";
        let updateUri = "/api/device/" + sourceDevice
            + "/layer/" + sourceLayer
            + "/key/" + sourceKey
            + "/" + operation + "/" + destinationKey;
        fetch(updateUri).then(() => {
            this.keyGrid.updateKey(sourceKey);
            this.keyGrid.updateKey(destinationKey);
        });
    }

    onKeyGridKeyClick(keyModel) {
        if (keyModel.isBound) {
            this.fetchAndBindKey(keyModel);
            this.propertiesCard.classList.add("active")
            this.createCard.classList.remove("active")
        } else {
            this.propertiesCard.classList.remove("active");
            this.createCard.classList.add("active");
        }
    }

    onSaveKeyClick(event) {
        this.unbindBehaviors();
        this.unbindPropertyEditors();
        let activeKey = this.keyGrid.activeKey;
        let updateUri = "/api/device/" + activeKey.device
            + "/layer/" + activeKey.layer
            + "/key/" + activeKey.key;

        fetch(updateUri, {
            method: 'PUT', headers: {
                'Content-Type': 'text/json'
            },
            body: JSON.stringify(this.activeModel)
        }).then(response => {
            return response.json();
        }).then(data => {
            let context = this;
            context.keyGrid.updateKey(activeKey.key);
            
        });
    }

    onDeleteKeyClick(event) {
        let activeKey = this.keyGrid.activeKey;
        let updateUri = "/api/device/" + activeKey.device
            + "/layer/" + activeKey.layer
            + "/key/" + activeKey.key;

        fetch(updateUri, {method: 'DELETE'}).then(() => {
            this.keyGrid.updateKey(activeKey.key);
        });
    }


    fetchAndBindKey(keyModel) {
        fetch("/api/device/" + keyModel.device + "/layer/" + keyModel.layer + "/key/" + keyModel.key)
            .then(response => {
                return response.json();
            })
            .then(keyModel => {
                this.activeModel = keyModel;
                this.bindBehavior(keyModel);
                let keyPropertiesPromise = this.getProperties("KeyBehavior", keyModel.Behavior["Type"]);
                let iconPropertiesPromise = this.getProperties("IconTemplate", keyModel.Behavior["Type"]);
                Promise.all([keyPropertiesPromise, iconPropertiesPromise]).then((properties) => {
                    this.bindPropertyEditors(keyModel, properties);
                    this.bindAddPropertyEditors(keyModel, properties)
                })
            });
    }

    bindAddPropertyEditors(keyMode, properties) {
        for (let propertyGroup in properties) {
            for (let property in properties[propertyGroup].properties) {
                let selectOption = document.createElement('option')
                let currentProp = properties[propertyGroup].properties[property];
                selectOption.innerText = currentProp.friendlyName;
                selectOption.value = currentProp.key;
                this.addPropertyNameSelect.appendChild(selectOption);
            }
        }
    }

    getProperties(parentTypeName, typeName) {
        return fetch("/api/property/" + parentTypeName + "/" + typeName)
            .then(response => {
                return response.json()
            })
    }


    bindBehavior(keyModel) {
        this.behaviorSelect.value = keyModel.Behavior.Type;
        let behaviorActionsUri = "/api/behavior/" + keyModel.Behavior.Type + "/actions";
        let actionsUri = "/api/action";
        fetch(behaviorActionsUri)
            .then(response => {
                return response.json();
            })
            .then(behaviorActionsObject => {
                fetch(actionsUri).then(response => {
                    return response.json();
                }).then(actionsArray => {
                    this.actionContainer.innerHTML = "";
                    behaviorActionsObject.actions.forEach(action => {
                        let actionElem = this.createElement_actionGroup(action, keyModel, actionsArray);
                        this.actionContainer.appendChild(actionElem);
                        let actionParameterUri = "/api/action/" + keyModel.Behavior.Actions[action.actionName].Type + "/parameters"
                        fetch(actionParameterUri)
                            .then(response => response.json())
                            .then(actionParams =>
                                actionParams.parameters.forEach(paramItem => {
                                    actionElem.appendChild(this.createElement_actionParamEditor(
                                        action,
                                        paramItem,
                                        keyModel.Behavior.Actions[action.actionName].Parameters[paramItem.name])
                                    );
                                }));
                    })

                })


            });
    }


    bindPropertyEditors(keyModel, propertyGroups) {
        this.tagContainer.innerHTML = "";
        for (var i = 0; i < propertyGroups.length; i++) {
            let propertyGroup = propertyGroups[i];
            let propertyGroupElem = this.createElement_propertyGroupCard(propertyGroup)
            for (const propertyName in propertyGroup.properties) {
                if (keyModel["Properties"][propertyName] !== undefined) {
                    let propertyEditorElem = this.createElement_propertyEditor(propertyName, propertyGroup.properties[propertyName].friendlyName,
                        keyModel);
                    propertyGroupElem.querySelector(".card-body").appendChild(propertyEditorElem);
                }
            }
            this.tagContainer.appendChild(propertyGroupElem);
        }
    }


    unbindPropertyEditors() {
        let properties = {};
        document.querySelectorAll(".keyTag").forEach(item => {
            properties[item.dataset.tag] = item.value;
        });
        this.activeModel["Properties"] = properties;
    }

    unbindBehaviors() {
        for (const actionName in this.activeModel.Behavior.Actions) {
            let actionSelectId = "action_" + actionName + "_type"
            let actionSelectElem = document.getElementById(actionSelectId);
            this.activeModel.Behavior.Actions[actionName].Type = actionSelectElem.value;
            for (const paramItem in this.activeModel.Behavior.Actions[actionName].Parameters) {
                let paramElement = document.getElementById("action_" + actionName + "_param_" + paramItem)
                this.activeModel.Behavior.Actions[actionName].Parameters[paramItem] = paramElement.value;
            }
        }
    }


    createElement_actionGroup(behaviorAction, keyModel, actionOptions) {
        let actionElem = document.createElement("div")
        let actionParamContainerId = "action_" + behaviorAction.actionName + "_params"
        let currentBehaviorAction = keyModel.Behavior.Actions[behaviorAction.actionName].Type;
        let html = "<div id='" + actionParamContainerId + "' class=\"form-group\">" +
            "<label for=\"keyJson\"> Action: " + behaviorAction.actionName + "</label>" +
            "<select id='action_" + behaviorAction.actionName + "_type' class='form-select'>";
        actionOptions.forEach(action => {
                       html += "<option " + (currentBehaviorAction == action ? "selected" : "") + " value=\""+ action + "\">" + action + "</option>" 
        });
        html += "</select>" +
            "</div>";
        actionElem.innerHTML = html;
        return actionElem;
    }

    createElement_actionParamEditor(action, param, currentValue) {
        let actionParamElem = document.createElement("div");
        actionParamElem.innerHTML = "" +
            "<div class=\"input-group mb-2\">" +
            "<span class=\"input-group-text\">" + param.name + "</span>" +
            "<input type=\"text\" id='action_" + action.actionName + "_param_" + param.name + "' class=\"form-control\" value=\"" + currentValue + "\">" +
            "</div>"
        return actionParamElem;
    }

    createElement_propertyEditor(propertyName, friendlyName, keyModel) {
        let propertyEditorElem = document.createElement("div");
        propertyEditorElem.innerHTML =
            "<div class='form-group mb-2'>" +
            "<label for=\"keyId\">" + friendlyName + "</label>" +
            "<input type=\"text\" id=\"tag_" + propertyName + "\" data-tag=\"" + propertyName + "\" class=\"form-control keyTag\" value=\"" + keyModel["Properties"][propertyName] + "\">" +
            "</div>";
        return propertyEditorElem;
    }

    createElement_propertyGroupCard(propertyGroup) {
        let propertyGroupElem = document.createElement("div");
        propertyGroupElem.innerHTML =
            "<div class=\"card mb-2\">\n" +
            "    <div class=\"card-header\">\n" +
            "        " + propertyGroup.friendlyName + "\n" +
            "    </div>\n" +
            "    <div class=\"card-body\">\n" +
            "    </div>\n" +
            "</div>\n";
        return propertyGroupElem;
    }
}