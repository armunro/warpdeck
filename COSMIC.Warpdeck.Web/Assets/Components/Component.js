export default {
    props: {
        deviceId: String,
        layerId: String,
        keyId: String
    },
    data() {
        return {
            isActive: false,
            cacheKey: null,
            mouseDownTime: null
        }
    },
    methods: {
        calculateIconUrl() {
            return `/api/device/${this.deviceId}/layer/${this.layerId}/icon/${this.keyId}?unq=${this.cacheKey}`;
        },
        activate() {
            this.isActive = true;
        },
        deactivate() {
            this.isActive = false;
        },
        refresh() {
            this.cacheKey = Date.now();
        },
        onComponentIconClick(item) {
            this.$emit('component-click', item);
        },
        onMouseDown(event){
            console.log('onMouseDown event: ' + JSON.stringify(event));
            this.mouseDownTime = Date.now();
        },
        onMouseUp(event){
            if (Date.now() - this.mouseDownTime > 500) {
                this.$emit('component-touch-press', this);
            } else {
                this.$emit('component-touch-hold', this);
            }
        }
    },
    mounted() {
        this.refresh()
    },
    template: `
      <div id="component_{{this.keyId}}" :class="{ active: isActive}" class="col m-0 p-0 keyMapKey">
        <img alt="" class="img-fluid me-2 mb-2 keyMapKeyImage"
             @click="onComponentIconClick(this)"
             @mousedown="onMouseDown"
             @mouseup="onMouseUp"
             :src="calculateIconUrl()"
             draggable="true"
        />
      </div>

    `
}