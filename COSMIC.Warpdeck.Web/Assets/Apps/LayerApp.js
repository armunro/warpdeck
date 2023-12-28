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
            editingDeviceId: null,
            editingLayerId: null,
            editingComponentId: null,
            editingIconProperties: null,
            editingComponentProperties: null,
        }
    }, methods: {
        componentClick(component) {
            let keyPropertiesPromise = this.api.getTypeProperties("Object", "ButtonBehavior");
            let iconPropertiesPromise = this.api.getTypeProperties("IconTemplate", "PressAndHold");
            Promise.all([keyPropertiesPromise, iconPropertiesPromise]).then((properties) => {
                this.editingComponentProperties = properties[1].properties;
                this.editingIconProperties = properties[0].properties;
            })

            api.getDeviceLayerComponent(component.deviceId, component.layerId, component.keyId).then(x => {
                this.editingComponent = x;
                this.editingDeviceId = component.deviceId;
                this.editingLayerId = component.layerId;
                this.editingComponentId = component.keyId;
                
            });
        }, componentSave(component) {
            this.api.saveDeviceLayerComponent(this.editingDeviceId, this.editingLayerId, this.editingComponentId, component).then();
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

