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
        },
        onKeyDragOver(event) {
            event.preventDefault();
        },
        onKeyDragStart(event) {
            let keyDiv = event.target.parentNode
            event.dataTransfer.setData("deviceId", this.deviceId);
            event.dataTransfer.setData("layerId", this.layerId);
            event.dataTransfer.setData("componentId", this.keyId);
        },
        onKeyDrop(event) {
            event.preventDefault();
            let sourceDevice = event.dataTransfer.getData("deviceId");
            let sourceLayer = event.dataTransfer.getData("layerId");
            let sourceKey = event.dataTransfer.getData("componentId");
            let destinationKey = event.target.dataset.index;
            let isCopy = event.ctrlKey;
            this.$emit("component-drag-drop", {sourceDevice, sourceLayer, sourceKey, destinationKey, isCopy})
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
             @dragstart="onKeyDragStart"
             @dragover="onKeyDragOver"
             @drop="onKeyDrop"
             :src="calculateIconUrl()"
             :data-index.attr="keyId"
             draggable="true"
        />
      </div>

    `
}