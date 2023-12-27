import {createApp} from 'https://unpkg.com/vue@3/dist/vue.esm-browser.js'
import WdComponent from "/Components/Component.js"
import WdComponentGrid from "/Components/ComponentGrid.js"
import WdEditComponentForm from "/Components/Forms/EditComponentForm.js"

let api;

let app = createApp({
    data() {
        return {
            api: null,
        }
    },
    methods: {
    
    },
    mounted() {
        api = new WarpdeckApi("http://localhost:4300/api")
        this.api = api;

    },
});


app.component("wdcomponent", WdComponent);
app.component("wdcomponentgrid", WdComponentGrid);
app.component("wdeditcomponentform", WdEditComponentForm);
app = app.mount("#layerApp");
document.app = app;

