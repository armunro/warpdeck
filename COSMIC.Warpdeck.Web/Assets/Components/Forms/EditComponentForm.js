

export default {
    props: {
        componentmodel: Object,
        keyproperties: Object,
        iconproperties: Object,
    },
    data() {
        return {}
    },
    methods: {
        getJson(){
            return JSON.stringify(this.componentmodel);
        },
        bindComponent(){
          
        },
        formatId(test){
            return "test_" + test
        }
        
    },
    mounted() {
        this.bindComponent();
    },
    template: `
      <div id="propertiesCard" class="active">
        <div class="card mb-2">
          <div class="card-header">
            Behavior
          </div>
          <div class="card-body">
            <!--<text>{{getJson()}}</text>-->
            <select id="behaviorSelect" class="form-select d-none">
              <option value="PressAndHold">Press And Hold</option>
            </select>
            <div id="actionContainer" class="form-group">
              <div>
                <div id="action_Press_params" class="form-group">
                  <div class="input-group mb-2"><span class="input-group-text">Press Action</span><select
                      id="action_Press_type" class="form-select">
                    <option value="PasteAction">PasteAction</option>
                    <option value="WebLaunchAction">WebLaunchAction</option>
                    <option value="ManageWindowAction">ManageWindowAction</option>
                    <option value="LauncherAction">LauncherAction</option>
                    <option selected="" value="KeyMacro">KeyMacro</option>
                  </select></div>
                </div>
                <div>
                  <div class="form-group">
                    <div class="input-group ps-4 mb-2"><span class="input-group-text">keys</span><input type="text"
                                                                                                        id="action_Press_param_keys"
                                                                                                        class="form-control"
                                                                                                        value="(mediaplaypause)">
                    </div>
                  </div>
                </div>
              </div>
              <div>
                <div id="action_Hold_params" class="form-group">
                  <div class="input-group mb-2"><span class="input-group-text">Hold Action</span><select
                      id="action_Hold_type" class="form-select">
                    <option value="PasteAction">PasteAction</option>
                    <option value="WebLaunchAction">WebLaunchAction</option>
                    <option value="ManageWindowAction">ManageWindowAction</option>
                    <option value="LauncherAction">LauncherAction</option>
                    <option value="KeyMacro">KeyMacro</option>
                  </select></div>
                </div>
              </div>
            </div>

            <form class="form">
              <div id="tagContainer" class="form-group">
                <div>
                  <div class="card mb-2">
                    <div class="card-header">
                      Properties
                    </div>
                    <div class="card-body">
                      <div>
                        <div class="form-group mb-2" v-for="property in this.keyproperties">
                          <label for="">{{ property.friendlyName }}</label><input type="text" :id="formatId(property.key)"
                                                                                  v-model="componentmodel.Properties[property.key]"
                                                                                  class="form-control keyTag">
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
                <div>
                  <div class="card mb-2">
                    <div class="card-header">
                      Properties
                    </div>
                    <div class="card-body">
                      <div>
                        <div class="form-group mb-2" v-for="property in this.iconproperties">
                          <label for="">{{ property.friendlyName }}</label><input type="text"
                                                                                  
                                                                                  v-model="componentmodel.Properties[property.key]"
                                                                                  class="form-control keyTag" value="">
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
              <div class="input-group">
                <select class="form-select" id="addPropertyNameSelect">
                  <option value="Category">Category</option>
                  <option value="Text">Text</option>
                  <option value="BackgroundColor">Background Color</option>
                  <option value="IconColor">Graphic Color</option>
                  <option value="Icon">Icon</option>
                  <option value="AccentColor">Accent Color</option>
                </select>
                <input type="text" id="addPropertyValueTextBox" class="form-control">
                <button class="btn btn-outline-secondary" id="addPropertyButton" type="button">
                  Add
                  Property
                </button>
              </div>
              <div class="form-group">
                <button id="saveKey" type="button" class="btn btn-primary">Save</button>
                <button id="deleteKey" type="button" class="btn btn-primary">Delete</button>
              </div>
            </form>
          </div>
        </div>
      </div>
    `
}
