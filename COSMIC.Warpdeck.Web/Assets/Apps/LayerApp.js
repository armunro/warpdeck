import {createApp} from 'https://unpkg.com/vue@3/dist/vue.esm-browser.js'
import WdComponent from "/Components/Component.js"
import WdComponentGrid from "/Components/ComponentGrid.js"
import WdEditComponentForm from "/Components/Forms/EditComponentForm.js"

let api;

let app = createApp({
    data() {
        return {
            api: null,
            editingComponent: null,
            editingComponent2: null,
            editingDeviceId: null,
            editingLayerId: null,
            editingComponentId: null,
            editingIconProperties: null,
            editingComponentProperties: null,
            editingTriggers:null
            
        }
    }, methods: {
        componentClick(component) {
            let keyPropertiesPromise = this.api.getTypeProperties("Object", "ButtonBehavior");
            let iconPropertiesPromise = this.api.getTypeProperties("IconTemplate", "PressAndHold");
            let triggersPromise = this.api.getComponentTriggers();
            Promise.all([keyPropertiesPromise, iconPropertiesPromise]).then((properties) => {
                this.editingComponentProperties = properties[1].properties;
                this.editingIconProperties = properties[0].properties;
            })
            
            triggersPromise.then(x=> this.editingTriggers = x.actions)
            

            api.getDeviceLayerComponent(component.deviceId, component.layerId, component.keyId).then(x => {
                this.editingComponent = x;
                this.editingComponent2 = component;
                this.editingDeviceId = component.deviceId;
                this.editingLayerId = component.layerId;
                this.editingComponentId = component.keyId;

            });
        }, componentSave: function (component) {
            this.api.saveDeviceLayerComponent(this.editingDeviceId, this.editingLayerId, this.editingComponentId, component)
                .then(() => this.editingComponent2.refresh());

        }
    }, mounted() {


    },
});

api = new WarpdeckApi("http://localhost:4300/api")

app.component("wdcomponent", WdComponent);
app.component("wdcomponentgrid", WdComponentGrid);
app.component("wdeditcomponentform", WdEditComponentForm);
app = app.mount("#layerApp");
window.app = app;
window.app.api = api;

