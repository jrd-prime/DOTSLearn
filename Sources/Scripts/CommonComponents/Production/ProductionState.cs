namespace Sources.Scripts.CommonComponents.Production
{
    public enum ProductionState
    {
        /// <summary>
        /// Default state in which production states are not processed
        /// </summary>
        NotEnoughProducts = 0,

        /// <summary>
        /// The state when there are enough products in warehouse to start production
        /// </summary>
        EnoughProducts = 1,

        /// <summary>
        /// The state of preparation for production
        /// </summary>
        Init = 2,

        /// <summary>
        /// The state in which production starts
        /// </summary>
        Started = 3,

        /// <summary>
        /// The state in which data is updated and changed during the production process
        /// </summary>
        InProgress = 4,

        /// <summary>
        /// A condition that indicates that production has been suspended for some reason and production is awaiting resumption
        /// </summary>
        Stopped = 5,

        /// <summary>
        /// A state that indicates the end of production and performs final actions
        /// </summary>
        Finished = 6,
    }
}