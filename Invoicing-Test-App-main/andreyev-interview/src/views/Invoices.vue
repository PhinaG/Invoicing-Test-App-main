<template>
  <br>  
  <div class="home">
    <form @submit.prevent>
      <label for="invoices">Create a new invoice</label>
      <div class="container">
        <div class="spacer"><input type="text" name="invoices" v-model="state.description" placeholder="Description" /></div>
        <div class="spacer"><input type="text" name="invoices" v-model="state.client" placeholder="Client's Name" /></div>
        <div class="spacer"><input type="text" name="invoices" v-model="state.couponCode" placeholder="Coupon Code, if any" /></div>
        <div class="spacer"><button class="create-button" @click="createInvoice">Create Invoice</button></div>
      </div>
    </form>

    <hr />

     <table>
      <thead>
        <th>ID</th>
        <th>Description</th>
        <th>Client</th>
        <th>Invoice Link</th>
        <th>Total value</th>
        <th>Total Billed Amount</th>
        <th>Coupon Code</th>
        <th>Actions</th>
      </thead>
      <tbody>
        <tr v-for="invoice in state.invoices" :key="invoice.id">
          <td>{{invoice.id}}</td>
          <td>{{invoice.description}}</td>
          <td>{{invoice.client.name}}</td>
          <td>
            <router-link :to="{ name: 'Invoice', params: { id: invoice.id, clientName: invoice.client.name, couponCode:invoice.couponCode }}">
              Open
            </router-link>
          </td>
           <td>{{invoice.totalValue}}</td>
          <td>{{invoice.totalBillableValue}}</td>
          <td>{{invoice.couponCode}}</td>
          <td>
            <button :id="invoice.id" @click="deleteInvoice(invoice.id)" class="error-button" >Delete</button>
           
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
import { ref, defineComponent, onMounted, reactive } from 'vue';

// @ is an alias to /src
export default defineComponent({
  name: 'Invoices',

  setup() {
    const state = reactive({
      invoices: [],
      description: "",
      couponCode:"",
      client:"",
      message : ref<string | null>(null)
    })

    function fetchInvoices(message: string = '') {
      fetch("http://localhost:5000/invoices", {
        method: "GET",
        headers: {
          "Content-Type": "application/json"
        },
      }).then((response) => {
        if(response.ok){
          response.json().then(invoices => (state.invoices = invoices.invoices));
          if(message != ''){
            alert(message);
          }           
        }
        
      })
    }

    function createInvoice() {
      fetch("http://localhost:5000/invoices", {
        method: "PUT",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          client: state.client,
          description: state.description,
          couponCode: state.couponCode
        })
      }).then((response) => {
        if(response.ok){          
          fetchInvoices(`A new invoice has been created to client ${state.client}`);
        }       
      })
    }

    function deleteInvoice(id:Number){
      fetch(`http://localhost:5000/invoices/${id}`, {
        method: "DELETE",
        headers: {
          "Content-Type": "application/json"
        }
      }).then((response) => {
        if(response.ok){          
          fetchInvoices(`An invoice with ID ${id} has been removed`);
        }        
      })
    }  

    onMounted(fetchInvoices)

    return {state, createInvoice, deleteInvoice}
  }
});
</script>
