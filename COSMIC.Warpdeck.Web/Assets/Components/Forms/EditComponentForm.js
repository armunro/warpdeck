export default {
    props: {
        componentmodel: Object,
        keyproperties: Object,
        iconproperties: Object,
        triggers: Object

    },
    data() {
        return {}
    },
    methods: {
        getJson() {
            return JSON.stringify(this.componentmodel);
        },
        saveComponent() {
            this.$emit('component-save', this.componentmodel);
            this.$emit('component-reload', this.componentmodel);
        },
        formatId(test) {
            return "test_" + test
        }

    },
    mounted() {
    },
    template: `
      <div id="propertiesCard" class="active" v-if="componentmodel !== null">
        <div class="card mb-2">
          <div class="card-header">
            Behavior
          </div>
          <div class="card-body">
            <select id="behaviorSelect" class="form-select d-none">
              <option value="PressAndHold">Press And Hold</option>
            </select>
            <div id="actionContainer" class="form-group">
              <div>
                <div v-for="trigger in triggers" class="form-group">
                  <div class="input-group mb-2" v-if="componentmodel.Actions[trigger.actionName]"><span class="input-group-text">{{ trigger.actionName }}
                    Action</span><select
                      v-model="componentmodel.Actions[trigger.actionName].Type" class="form-select">
                    <option value="PasteAction">PasteAction</option>
                    <option value="WebLaunchAction">WebLaunchAction</option>
                    <option value="ManageWindowAction">ManageWindowAction</option>
                    <option value="LauncherAction">LauncherAction</option>
                    <option selected="" value="KeyMacro">KeyMacro</option>
                  </select>
                    <div>
                      <div class="form-group" v-for="(param, index) in componentmodel.Actions[trigger.actionName].Parameters">
                        <div class="input-group ps-4 mb-2">
                          <span class="input-group-text">{{index}}</span>
                          <input type="text"
                                 id="action_Press_param_keys"
                                 class="form-control"
                                 v-model="componentmodel.Actions[trigger.actionName].Parameters[index]">
                        </div>
                      </div>
                    </div>
                  </div>
                  
                </div>

              </div>
            </div>

            <form class="form">
              <div id="tagContainer" class="form-group">
                <div>
                  <div class="card mb-2">
                    <div class="card-header">
                      Icon Properties
                    </div>
                    <div class="card-body">
                      <div>
                        <template v-for="property in this.keyproperties">
                          <div v-if="componentmodel.Properties[property.key]" class="form-group mb-2">
                            <label for="">{{ property.friendlyName }}</label><input type="text"
                                                                                    :id="formatId(property.key)"
                                                                                    v-model="componentmodel.Properties[property.key]"
                                                                                    class="form-control keyTag">
                          </div>
                        </template>

                      </div>
                    </div>
                  </div>
                </div>
                <div>
                  <div class="card mb-2">
                    <div class="card-header">
                      Component Properties
                    </div>
                    <div class="card-body">
                      <div>
                        <div class="form-group mb-2" v-for="property in this.iconproperties">
                          <label for="">{{ property.friendlyName }}</label>
                          <input type="text"
                                 v-model="componentmodel.Properties[property.key]"
                                 class="form-control keyTag" >
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <div class="form-group">
                <button id="saveKey" @click="saveComponent" type="button" class="btn btn-primary">Save</button>
              </div>
            </form>
          </div>
        </div>
      </div>
    `
}
