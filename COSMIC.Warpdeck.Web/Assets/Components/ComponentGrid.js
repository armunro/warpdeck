export default {
    props: {
        deviceid: String,
        layerid: String,
        columns: String,
        rows: String
    },
    data() {
        return {
            activeComponent: null
        }
    },
    methods: {
        range(end) {
            return Array.from({length: end}, (_, i) => i + 1)
        },
        
        component_clicked(clickedComponent) {
            if (this.activeComponent)
                this.activeComponent.deactivate();
            clickedComponent.activate();
            this.activeComponent = clickedComponent;
            this.$emit('component-click', this.activeComponent);
        },
        component_refresh_request(component){
            this.activeComponent.refresh();
        },
        component_touch_press(e){
            this.$emit('component-touch-press', e);
        },
        component_touch_hold(e){
            this.$emit('component-touch-hold', e);
        }
    },
    mounted() {
    },
    template: `
      <div class="row" v-for='r in range(this.rows)'>
        <wdcomponent :deviceId="this.deviceid" :layerId="this.layerid"
                     :keyId="(((r-1) * this.columns) + (c-1)).toString()"
                     @component-click="component_clicked"
                     @component-touch-press="component_touch_press"
                     @component-touch-hold="component_touch_hold"
                     v-for="c in range(this.columns)"></wdcomponent>
      </div>
    `
}
