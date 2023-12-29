export default {
    props: {
        deviceId: String,
        layerId: String,
        keyId: String
    },
    data() {
        return {
            isActive: false,
            cacheKey: null
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
        onDragStart(event){
            console.log('onDragStart event: ' + JSON.stringify(event));
        },
        onDragDrop(event){
            console.log('onDragDrop event: ' + JSON.stringify(event));
        },
        onDragOver(event){
            //console.log('onDragOver event: ' + JSON.stringify(event));
        },
        onMouseDown(event){
            console.log('onMouseDown event: ' + JSON.stringify(event));
        },
        onMouseUp(event){
            console.log('onMouseUp event: ' + JSON.stringify(event));
        }
    },
    mounted() {
        this.refresh()
    },
    template: `
      <div id="component_{{this.keyId}}" :class="{ active: isActive}" class="col m-0 p-0 keyMapKey" >
        <img alt="" class="img-fluid me-2 mb-2 keyMapKeyImage" @click="onComponentIconClick(this)" draggable="true"
             @dragstart="onDragStart" @dragend="ondrop" @drop="onDragDrop" @dragover="onDragOver" @mousedown="onMouseDown" @mouseup="onMouseUp"
             :src="calculateIconUrl()"/>
      </div>
      
    `
}