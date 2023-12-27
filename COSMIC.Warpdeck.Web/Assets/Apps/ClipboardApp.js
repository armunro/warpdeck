import {createApp} from 'https://unpkg.com/vue@3/dist/vue.esm-browser.js'


let api;

let app = createApp({
    data() {
        return {
            api: null,
            clips: [{text: ''}]
        }
    },
    methods: {
        loadClips() {
            api.getClips().then(value => this.clips = value)
        },
        triggerAction(actionTypeName, parametersString) {
            api.triggerAction(actionTypeName, parametersString).then(response => console.log(response) )
        }
    },
    mounted() {
        api = new WarpdeckApi("http://localhost:4300/api")
        this.api = api;
        this.loadClips();
    },
});


//app.component("fptasklist", FpTaskList);
app = app.mount("#clipboardApp");
document.app = app;

