<template>
    <br>
    <div class="home">
      <form @submit.prevent>
        <label for="clients">Create a new client</label>
        <div class="container">
          <div class="spacer"><input type="text" name="clients" v-model="state.name" placeholder="Name" /></div>
          <div class="spacer"><button class="create-button" @click="createClient">Create a new Client</button></div>
        </div>
      </form>
  
      <hr />
  
       <table>
        <thead>
          <th>ID</th>
          <th>Name</th>
          <th>Actions</th>
        </thead>
        <tbody>
          <tr v-for="client in state.clients" :key="client.id">
            <td>{{client.id}}</td>
            <td> <input type="text" name="name" v-model="client.name" placeholder="Name"  @change="updateClient(client)"/></td>
            <td>
                <button :id="client.id" @click="deleteClient(client.id, client.name)" class="error-button" >Delete</button>
            </td>       
          </tr>
        </tbody>
      </table>
    </div>
  </template>
  
  <style scoped>
  .container {
    display: flex; /* Align entries vertically */
    align-items: center;    /* Center horizontally */
  }
  
  .entry {
    background-color: #f0f0f0;
    border: 1px solid #ccc;
    padding: 16px;
    margin: 8px;
    width: 80%;
    height:50%;
    text-align: center;
  }
  
  .spacer {
    flex: 1;/* Adjust spacer height as needed */
  }
  
  .create-button{
    color:white; 
    background-color: rgb(73, 73, 194);
  }
  
  .success-button{
    color:white; 
    background-color:rgb(67, 179, 67);
  }
  
  .error-button{
    color:white; 
    background-color:rgb(211, 86, 86);
  }
  .create-button:hover {
  background-color: #6f7275;
}

.success-button:hover {
  background-color: #6f7275;
}

.error-button:hover {
  background-color: #6f7275;
}
  </style>
  
  <script lang="ts">
  import { defineComponent, onMounted, reactive } from 'vue';
  // @ is an alias to /src
  
  export default defineComponent({
    name: 'Clients',
    setup() {
      const state = reactive({
        clients: [],
        name: ""
      })
  
      function fetchClients(message: string = '') {
        fetch("http://localhost:5000/clients", {
          method: "GET",
          headers: {
            "Content-Type": "application/json"
          },
        }).then((response) => {
          response.json().then(clients => (state.clients = clients.clientList))
          if(message != ''){
            alert(message);
          }
        })
      }
  
      function createClient() {
        fetch("http://localhost:5000/clients", {
          method: "POST",
          headers: {
            "Content-Type": "application/json"
          },
          body: JSON.stringify({
            name: state.name
          })
        }).then((response) => {
        if(response.ok){          
          fetchClients(`A new client: ${state.name} has been created`);
        }
        
        })
    }

      function updateClient(client:any){
        let id = client.id;
        fetch(`http://localhost:5000/clients/${id}`, {
          method: "PATCH",
          headers: {
            "Content-Type": "application/json"
          },
          body: JSON.stringify({
            name:client.name
          })
        }).then((response) => {
        if(response.ok){          
          fetchClients(`A client: ${client.name} has been updated`);
        }
       
        })
      }
  
      function deleteClient(id:Number, name:string){
        fetch(`http://localhost:5000/clients/${id}`, {
          method: "DELETE",
          headers: {
            "Content-Type": "application/json"
          }
        }).then((response) => {
        if(response.ok){          
          fetchClients(`A client: ${name} has been removed`);
        }        
      })
        
      }    
      onMounted(fetchClients)
  
      return {state, createClient,updateClient, deleteClient}
    }
});
  
  </script>
  