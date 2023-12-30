import {createApp} from 'https://unpkg.com/vue@3/dist/vue.esm-browser.js'
import WdComponent from "/Components/Component.js"
import WdComponentGrid from "/Components/ComponentGrid.js"

let api;

let app = createApp({
    data() {
        return {
            api: null
        }
    },
    methods: {
        pressed(e) {
            this.api.triggerComponentHold(e.deviceId, e.keyId).then()
            
        },
        held(e) {
            this.api.triggerComponentPress(e.deviceId, e.keyId).then()
        }
    },
    mounted() {
        api = new WarpdeckApi("http://localhost:4300/api")
        this.api = api;

    }
});


app.component("wdcomponent", WdComponent);
app.component("wdcomponentgrid", WdComponentGrid);
app = app.mount("#touchDeviceApp");
document.app = app;

